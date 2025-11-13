using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.LogicaDeNegocio.Productos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registrar servicios
builder.Services.AddScoped<IListarProductos, ListarProductos>();
builder.Services.AddScoped<IObtenerProducto, ObtenerProducto>();
builder.Services.AddScoped<ICrearProducto, CrearProducto>();
builder.Services.AddScoped<IActualizarProducto, ActualizarProducto>();
builder.Services.AddScoped<IEliminarProducto, EliminarProducto>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
   // app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();           
app.UseAuthorization();

//app.UseExceptionHandler("/Error/InternalServerError");
//app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllers();       
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
