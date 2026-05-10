using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineClothingStore.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Activam MVC: Controllers + Views
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Activam memoria pentru sesiune
builder.Services.AddDistributedMemoryCache();

// Configuram sesiunea pentru cos, wishlist etc.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DatabaseSeeder.Seed(context);
}

// Pagina de erori pentru dezvoltare
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Permite incarcarea fisierelor din wwwroot:
// css, imagini, js etc.
app.UseStaticFiles();

app.UseRouting();

// Sesiunea trebuie sa fie dupa UseRouting
// si inainte de MapControllerRoute
app.UseSession();

app.UseAuthorization();

// Ruta principala a site-ului tau
// localhost:5000 va deschide StoreController -> Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");

app.Run();
