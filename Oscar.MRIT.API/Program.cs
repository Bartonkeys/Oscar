using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Oscar.MRIT.Core.Configuration;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Features.MRITIntegration.Common;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.DI;
using Oscar.Core.Providers;
using Oscar.Infrastructure.Providers;
using Oscar.Mrit.Data;
using UserProvider = Oscar.Mrit.Features.Common.UserProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.UseSqlServer(builder.Configuration);

builder.Services.AddDbContext<FelixMritContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("FelixMrit")));
builder.Services.AddSingleton<Oscar.Mrit.Features.Common.IUserProvider, UserProvider>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAutoMapper(typeof(UpdateMatchStatusCommand).GetTypeInfo().Assembly);
builder.Services.AddMediatR(typeof(UpdateMatchStatusCommand).GetTypeInfo().Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMatchStatusCommand>(ServiceLifetime.Transient);
builder.Services.AddScoped<MritMapperFactory>();

builder.Services.Configure<BlackListDto>(builder.Configuration.GetSection("Blacklist"));
builder.Services.Configure<BatchSettings>(builder.Configuration.GetSection("BatchSettings"));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MRIT Felix API", Version = "v1" });

    //var filePath = Path.Combine(AppContext.BaseDirectory, "Felix.xml");
    //c.IncludeXmlComments(filePath);

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Description = "Felix API Key",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            new[] { "readAccess", "writeAccess" }
        }
    });
});

//const string providerName1 = "InMemory1";
//builder.Services.AddEFSecondLevelCache(options =>
//    options.UseEasyCachingCoreProvider(providerName1, isHybridCache: false).DisableLogging(true).UseCacheKeyPrefix("EF_")
//);

//// Add an in-memory cache service provider
//// More info: https://easycaching.readthedocs.io/en/latest/In-Memory/
//builder.Services.AddEasyCaching(options =>
//{
//    // use memory cache with your own configuration
//    options.UseInMemory(config =>
//    {
//        config.DBConfig = new InMemoryCachingOptions
//        {
//            // scan time, default value is 60s
//            ExpirationScanFrequency = 60,
//            // total count of cache items, default value is 10000
//            SizeLimit = 100,

//            // enable deep clone when reading object from cache or not, default value is true.
//            EnableReadDeepClone = false,
//            // enable deep clone when writing object to cache or not, default value is false.
//            EnableWriteDeepClone = false,
//        };
//        // the max random second will be added to cache's expiration, default value is 120
//        config.MaxRdSecond = 120;
//        // whether enable logging, default is false
//        config.EnableLogging = false;
//        // mutex key's alive time(ms), default is 5000
//        config.LockMs = 5000;
//        // when mutex key alive, it will sleep some time, default is 300
//        config.SleepMs = 300;
//    }, providerName1);
//});

builder.Services.AddCors(o => o.AddPolicy("FelixPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddHttpClient("mrit", c =>
{
    //c.BaseAddress = new Uri("https://staging.compact-data.co.uk:8079");
    c.BaseAddress = new Uri("https://localhost:44397");
    c.DefaultRequestHeaders.Add("ApiKey", builder.Configuration.GetValue<string>("MritApiKey"));
});

var app = builder.Build();

app.UseSwagger();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
