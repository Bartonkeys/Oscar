using System.Reflection;
using MudBlazor.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Oscar.DI;
using Microsoft.Extensions.Azure;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Data.Context;
using Microsoft.AspNetCore.Components;
using Oscar.Blazor.Library.Shared;
using Microsoft.AspNetCore.Http.Features;
using Oscar.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Use a mock local authentication scheme in Development so the app can run without
// access to the Azure AD tenant. Set UseMockAuth=false in appsettings.Development.json
// to opt back into real Azure AD sign-in locally.
var useMockAuth = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue("UseMockAuth", true);

if (useMockAuth)
{
    builder.Services.AddAuthentication(DevAuthenticationHandler.SchemeName)
        .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevAuthenticationHandler>(
            DevAuthenticationHandler.SchemeName, _ => { });
}
else
{
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
}

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var configTimeoutSection = builder.Configuration.GetSection("SessionTimeoutMinutes");
var defaultTimeout = 2;//30; // Adjust default timeout period in minutes
//if (configTimeoutSection != null) int.TryParse(configTimeoutSection.Value, out defaultTimeout);

// Add services to the container.
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(o => { o.MaximumReceiveMessageSize = 10 * 1024 * 1024; })
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
        // Set session timeout to 30 minutes
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(defaultTimeout);
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(defaultTimeout);
    });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.UseSqlServer(builder.Configuration);

builder.Services.ConfigureFeatures(Assembly.GetExecutingAssembly());

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["oscarstorage"]);
    clientBuilder.AddQueueServiceClient(builder.Configuration["oscarstorage"]);
});

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDistributedMemoryCache(options =>
{
    options.SizeLimit = 4L * 1024 * 1024 * 1024; // 4GB
});

builder.Services.AddHttpClient("Oscar.Blazor");
builder.Services.AddControllersWithViews();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromMinutes(defaultTimeout); });

builder.Services.AddSingleton<SettingsModel>();
builder.Services.AddScoped<ReferenceDataService>();
builder.Services.AddScoped<OscarDataService>();


// Spinner
builder.Services.AddScoped<SpinnerService>();
builder.Services.AddScoped<SpinnerHandler>();
builder.Services.AddScoped(s =>
{
    SpinnerHandler spinnerHandler = s.GetRequiredService<SpinnerHandler>();
    spinnerHandler.InnerHandler = new HttpClientHandler();
    NavigationManager navManager = s.GetRequiredService<NavigationManager>();
    return new HttpClient(spinnerHandler) { BaseAddress = new Uri(navManager.BaseUri) };
});

//builder.Services.AddStackExchangeRedisCache(options => {
//    options.Configuration = builder.Configuration["oscarstorage:redis-cache"];
//});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 200 MB
});

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("OscarConnection")))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OscarContext>();
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseSession();
//app.UseSessionTimeoutMiddleware(TimeSpan.FromMinutes(defaultTimeout), "/dashboard");

app.Run();