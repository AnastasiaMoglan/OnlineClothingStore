using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Activăm MVC: Controllers + Views
builder.Services.AddControllersWithViews();

// Activăm memoria pentru sesiune
builder.Services.AddDistributedMemoryCache();

// Configurăm sesiunea pentru coș, wishlist etc.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Pagina de erori pentru dezvoltare
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Permite încărcarea fișierelor din wwwroot:
// css, imagini, js etc.
app.UseStaticFiles();

app.UseRouting();

// Sesiunea trebuie să fie după UseRouting
// și înainte de MapControllerRoute
app.UseSession();

app.UseAuthorization();

// Ruta principală a site-ului tău
// localhost:5000 va deschide StoreController -> Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");

app.Run();