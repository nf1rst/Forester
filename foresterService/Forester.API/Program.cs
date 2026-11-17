using Forester.API.Components;
using Forester.Application.Extensions;
using Forester.DAL.Extensions;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dbPath = Environment.GetEnvironmentVariable("FORESTER_DB_PATH") ??
    Path.Combine(builder.Environment.ContentRootPath, "forester-data", "forester.db"); //forester.API/*
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

var connectionString = $"Data Source={dbPath}";

builder.Services.SetDALServices(connectionString);
builder.Services.SetApplicationServices();


var keysPath = Environment.GetEnvironmentVariable("FORESTER_KEYS_PATH")
    ?? Path.Combine(builder.Environment.ContentRootPath, "keys"); //forester.API/*

builder.Services.AddDataProtection()
    .SetApplicationName("Forester")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

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
