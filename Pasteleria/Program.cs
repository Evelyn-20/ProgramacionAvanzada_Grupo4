using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.LogicaDeNegocio.Productos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registrar los servicios de Producto
builder.Services.AddScoped<IListarProductos, ListarProductos>();
builder.Services.AddScoped<IObtenerProducto, ObtenerProducto>();
builder.Services.AddScoped<ICrearProducto, CrearProducto>();
builder.Services.AddScoped<IActualizarProducto, ActualizarProducto>();
builder.Services.AddScoped<IEliminarProducto, EliminarProducto>();

// Configurar Session para login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseExceptionHandler("/Error/InternalServerError");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();