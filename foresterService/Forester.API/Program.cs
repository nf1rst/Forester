using Forester.API.Components;
using Forester.Application.Extensions;
using Forester.DAL.Data;
using Forester.DAL.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dataDir = builder.Configuration["FORESTER_DATA_DIR"]
    ?? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "forester-data"));


var dbPath = Path.Combine(dataDir, "forester.db");
var keysPath = Path.Combine(dataDir, "keys");


Directory.CreateDirectory(dataDir);
Directory.CreateDirectory(keysPath);

var connectionString = $"Data Source={dbPath}";

builder.Services.SetDALServices(connectionString);
builder.Services.SetApplicationServices();


builder.Services.AddDataProtection()
    .SetApplicationName("Forester")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ForesterDBContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
