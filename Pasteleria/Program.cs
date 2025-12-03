using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.LogicaDeNegocio.Productos;
using Pasteleria.LogicaDeNegocio.Pedidos;
using Microsoft.AspNetCore.Authentication;
using Pasteleria.AccesoADatos.Modelos;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR CONTROLADORES CON VISTAS Y JSON
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

builder.Services.AddScoped<Contexto>();

// CONFIGURAR SESIONES PRIMERO (Para Carrito y validación de login)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".Pasteleria.Session";
});

// CONFIGURAR AUTENTICACIÓN CON COOKIES
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.Name = ".Pasteleria.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;

        // EVENTO: Validar sesión en cada request
        options.Events.OnValidatePrincipal = async context =>
        {
            var sessionKey = "SessionId";
            var sessionId = context.HttpContext.Session.GetString(sessionKey);
            var userSessionId = context.Principal?.FindFirst("SessionId")?.Value;

            // Si no hay session activa en el servidor, cerrar sesión
            if (string.IsNullOrEmpty(sessionId) || sessionId != userSessionId)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync("CookieAuth");
            }
        };
    });

// REGISTRAR SERVICIOS DE PRODUCTOS
builder.Services.AddScoped<IListarProductos, ListarProductos>();
builder.Services.AddScoped<IObtenerProducto, ObtenerProducto>();
builder.Services.AddScoped<ICrearProducto, CrearProducto>();
builder.Services.AddScoped<IActualizarProducto, ActualizarProducto>();
builder.Services.AddScoped<IEliminarProducto, EliminarProducto>();

// REGISTRAR SERVICIOS DE PEDIDOS
builder.Services.AddScoped<IListarPedidos, ListarPedidos>();
builder.Services.AddScoped<IObtenerPedido, ObtenerPedido>();
builder.Services.AddScoped<ICrearPedido, CrearPedido>();
builder.Services.AddScoped<IActualizarPedido, ActualizarPedido>();
builder.Services.AddScoped<IEliminarPedido, EliminarPedido>();
builder.Services.AddScoped<IBuscarProductosParaPedido, BuscarProductosParaPedido>();
builder.Services.AddScoped<ICalcularTotales, CalcularTotales>();
builder.Services.AddScoped<IGestionarEstadosPedido, GestionarEstadosPedido>();

var app = builder.Build();

// CONFIGURACIÓN DE ROTATIVA PDF
Rotativa.AspNetCore.RotativaConfiguration.Setup(
    app.Environment.ContentRootPath,
    "wwwroot/Rotativa"
);

// CONFIGURAR EL PIPELINE HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



