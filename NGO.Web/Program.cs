using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NGO.Common;
using NGO.Web.Infrastructure;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using NGO.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager ConfigurationManager = builder.Configuration;
IWebHostEnvironment Environment = builder.Environment;
var configRoot = new
{
    AppSettings = new AppSettings()
};
ConfigurationManager.Bind(configRoot);

// Add services to the container.
string connectionString = System.Environment.GetEnvironmentVariable("DATABASE_URL")
                          ?? builder.Configuration["AppSettings:ConnectionString"];

builder.Services.AddDbContext<NGOContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.RegisterDependency(configRoot.AppSettings);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
                      builder =>
                      {
                          builder.AllowAnyOrigin().
                                              AllowAnyMethod().
                                              AllowAnyHeader();
                                        
                      });
});

builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
}).CreateMapper());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NGO API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = null,
    });
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });

});
var key = Encoding.ASCII.GetBytes(configRoot.AppSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromDays(1),
    };
});
// In production, the Angular files will be served from this directory
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist/ClientApp";
});
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
//builder.Services.AddControllersWithViews()
//    .AddNewtonsoftJson(options =>
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyMethod().
                       AllowAnyHeader().
                       WithHeaders().
                       AllowCredentials().
                       SetIsOriginAllowed(origin =>
                       true));
}
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NGO API V1");
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

//if (!app.Environment.IsDevelopment())
//{
//    app.UseSpaStaticFiles();
//}

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist", "ClientApp"))
    });
}



#region Custom Middlewares
app.UseApplicationMiddleware();
#endregion

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    endpoints.MapControllers();
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    if (app.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start");
    }
    else
    {
        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist", "ClientApp"))
        };
    }
});

//app.MapFallbackToFile("index.html"); ;
//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "ClientApp";

//    if (app.Environment.IsDevelopment())
//    {
//        //spa.UseProxyToSpaDevelopmentServer("https://community-ngo.vercel.app/");

//        // NOTE: Disable above line and enable below line to trigger angular from dev server.
//        spa.UseAngularCliServer(npmScript: "start");
//    }
//     else
//    {
//        // In production, ensure the default document is set to index.html
//        spa.Options.DefaultPage = "/index.html"; // Ensures the app serves the index.html file
//    }
//});
app.Run();
