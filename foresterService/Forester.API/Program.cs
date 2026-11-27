using Forester.API.Components;
using Forester.Application.Extensions;
using Forester.Application.Helper;
using Forester.DAL.Data;
using Forester.DAL.Extensions;
using Forester.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ForesterDBContext>()
    .AddSignInManager();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/error";
});

var dataDir = builder.Configuration["FORESTER_DATA_DIR"]
    ?? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "forester-data"));

var dbPath = Path.Combine(dataDir, "forester.db");
var keysPath = Path.Combine(dataDir, "keys");

Directory.CreateDirectory(dataDir);
Directory.CreateDirectory(keysPath);

var connectionString = $"Data Source={dbPath}";

builder.Services.SetDALServices(connectionString);
builder.Services.SetApplicationServices();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddControllers();

builder.Services.AddDataProtection()
.PersistKeysToFileSystem(new DirectoryInfo(keysPath))
.SetApplicationName("Forester");

builder.Services.AddHttpClient<PredictionClient>(c => c.BaseAddress = new Uri(builder.Configuration["FASTAPI_URL"] ?? "http://localhost:8000/"));
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ForesterDBContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("~/login");
}).RequireAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
