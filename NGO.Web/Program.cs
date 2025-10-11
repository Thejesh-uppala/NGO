using System.Text;
using System.Threading.Channels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NGO.Common;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;
using NGO.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// Bind configurations
var configuration = builder.Configuration;

// Bind AppSettings from environment variables
var appSettings = new AppSettings();
configuration.Bind("AppSettings", appSettings);

// Add services to the container
string connectionString = appSettings.ConnectionString ?? throw new InvalidOperationException("The 'ConnectionString' environment variable is missing or null. Please set AppSettings__ConnectionString.");
string secret = appSettings.Secret ?? throw new InvalidOperationException("The 'Secret' environment variable is missing or null. Please set AppSettings__Secret.");


// builder.Services.AddDbContext<NGOContext>(options =>
//options.UseSqlServer(connectionString, sqlServerOptions =>
//    {
//        sqlServerOptions.CommandTimeout(60);
//        sqlServerOptions.EnableRetryOnFailure(
//            maxRetryCount: 5,
//            maxRetryDelay: TimeSpan.FromSeconds(5),
//            errorNumbersToAdd: null
//        );
//    }));

builder.Services.AddDbContext<NGOContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null
        );
    })
   );

builder.Services.AddMemoryCache();
builder.Services.AddDataProtection()
    .SetApplicationName("NGOApp")
    .PersistKeysToDbContext<NGOContext>();


builder.Services.AddScoped<IOrganizationConfigService, OrganizationConfigService>();
builder.Services.RegisterDependency(appSettings);
builder.Services.AddControllers();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});



var emailChannel = Channel.CreateUnbounded<EmailDataModel>();
builder.Services.AddSingleton(emailChannel);

// Register the background worker
builder.Services.AddHostedService<EmailBackgroundWorker>();


builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
}).CreateMapper());

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NGO API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new List<string>() }
    });
});

var key = Encoding.ASCII.GetBytes(secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

// Configure middleware
app.UseMiddleware<ApplicationMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .SetIsOriginAllowed(origin => true));
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NGO API V1");
});

// Serve static files from wwwroot
app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Fallback to index.html for SPA routing
app.MapFallbackToFile("/index.html"); // Adjusted path

app.Run();



