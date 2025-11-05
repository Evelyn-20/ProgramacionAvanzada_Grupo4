using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.LogicaDeNegocio.Productos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar los servicios de Producto
builder.Services.AddScoped<IListarProductos, ListarProductos>();
builder.Services.AddScoped<IObtenerProducto, ObtenerProducto>();
builder.Services.AddScoped<ICrearProducto, CrearProducto>();
builder.Services.AddScoped<IActualizarProducto, ActualizarProducto>();
builder.Services.AddScoped<IEliminarProducto, EliminarProducto>();


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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();