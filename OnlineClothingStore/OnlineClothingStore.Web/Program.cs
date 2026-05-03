using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Activam MVC: Controllers + Views
builder.Services.AddControllersWithViews();

// Activam sesiunea pentru Login / Logout si roluri:
// Customer, Cashier, Manager
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configurare pentru erori in productie
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Store/Error");
    app.UseHsts();
}

// Redirectionare catre HTTPS
app.UseHttpsRedirection();

// Permite folosirea fisierelor statice din wwwroot:
// CSS, imagini, JavaScript
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT:
// UseSession trebuie sa fie dupa UseRouting
// si inainte de UseAuthorization / MapControllerRoute
app.UseSession();

app.UseAuthorization();

// Ruta principala a site-ului
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");

app.Run();