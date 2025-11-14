using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StockLine_API;
using StockLine_API.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde .env
Env.Load();


builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockLine API",
        Version = "v1"
    });
});


// Usar la variable de entorno para la cadena de conexión
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<StockLineContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<SIMService>();
builder.Services.AddScoped<EnvioService>();
builder.Services.AddScoped<MovimientoStockService>();
builder.Services.AddScoped<ComercialService>();
builder.Services.AddScoped<AyuntamientoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<CategoriaService>();

var app = builder.Build();


app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        var expected = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin:password123"));
        if (authHeader != expected)
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("No autorizado");
            return;
        }
    }
    await next();
});


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockLine API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization(); 

app.MapControllers();

app.Run();
