using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Oscar.DI;
using System.Reflection;
using Alachisoft.NCache.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Oscar.Core.Extensions;
using Oscar.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var azureAdSection = builder.Configuration.GetSection("AzureAd");
var addSwagger = builder.Configuration.GetValue<bool>("AddSwagger");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(azureAdSection);

builder.Services.UseSqlServer(builder.Configuration);
//builder.Services.AddDbContext<OscarContext>(optionsBuilder => {
//    string cacheId = "oscarClusteredCache";
//    NCacheConfiguration.Configure(cacheId, DependencyType.SqlServer);
//    NCacheConfiguration.ConfigureLogger();
//    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("OscarConnection"));
//});
builder.Services.ConfigureFeatures(Assembly.GetExecutingAssembly());

builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

if (addSwagger)
{
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "OAuth2.0 Auth Code with PKCE",
            Name = "oauth2",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl =
                        new Uri(azureAdSection["AuthorizationUrl"]),
                    TokenUrl = new Uri(azureAdSection["TokenUrl"]),
                    Scopes = new Dictionary<string, string>
                    {
                        { azureAdSection["ScopeFullName"], "Access Oscar API" }
                    }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                new[] { azureAdSection["ScopeFullName"] }
            }
        });
    });
}
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["oscarstorage"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["oscarstorage"], preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

if (addSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();