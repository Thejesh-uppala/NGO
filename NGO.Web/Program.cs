using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NGO.Common;
using NGO.Web.Infrastructure;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NGO.Repository;

var builder = WebApplication.CreateBuilder(args);

// Bind configurations
ConfigurationManager ConfigurationManager = builder.Configuration;
IWebHostEnvironment Environment = builder.Environment;
var configRoot = new
{
    AppSettings = new AppSettings()
};
ConfigurationManager.Bind(configRoot);

// Add services to the container
string connectionString = System.Environment.GetEnvironmentVariable("DATABASE_URL")
                          ?? builder.Configuration["AppSettings:ConnectionString"];

builder.Services.AddDbContext<NGOContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.RegisterDependency(configRoot.AppSettings);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
}).CreateMapper());

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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

var key = Encoding.ASCII.GetBytes(configRoot.AppSettings.Secret);
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
        ClockSkew = TimeSpan.FromDays(1),
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
app.MapFallbackToFile("/ClientApp/index.html"); // Adjusted path

app.Run();
