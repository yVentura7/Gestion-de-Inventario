using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Caso1_Tarea.Models;
using Caso1_Tarea.Models.Enums;
using Caso1_Tarea.Repositories;
using Caso1_Tarea.Repositories.Interfaces;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Para manejar enums como strings en JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Para evitar referencias circulares
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configurar DbContext
builder.Services.AddDbContext<Lab5Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        o => o.MapEnum<TipoMovimiento>("tipo_movimiento")
            .MapEnum<EstadoPedido>("estado_pedido")));

// Registrar Unit of Work y Repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestión de Inventario",
        Version = "v1",
        Description = "API para gestionar productos, proveedores, pedidos y movimientos de inventario",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tu-email@example.com",
        }
    });

    // Incluir comentarios XML para documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar para mostrar enums como strings
    c.UseInlineDefinitionsForEnums();
});

// Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Habilitar Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Gestión de Inventario v1");
    c.RoutePrefix = string.Empty; // Para que Swagger sea la página principal
    c.DocumentTitle = "API de Gestión de Inventario - Documentación";
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Aplicar migraciones automáticamente (opcional)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Lab5Context>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();