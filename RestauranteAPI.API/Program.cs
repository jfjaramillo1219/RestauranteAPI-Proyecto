using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;

var builder = WebApplication.CreateBuilder(args);

// Agregamos el soporte para Controladores (Requisito de la rúbrica)
builder.Services.AddControllers();

// INYECCIÓN DE DEPENDENCIAS: Configuramos Entity Framework Core
builder.Services.AddDbContext<RestauranteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuramos Swagger para .NET 8
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- DATA SEEDER EXECUTION ---

// Create a temporary scope to get the database context
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RestauranteDbContext>();

    // Apply pending migrations automatically (useful when cloning repo)
    context.Database.Migrate();

    // Execute the seed data method
    DataSeeder.SeedData(context);
}
// -----------------------------
// Configuramos el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Habilitamos la interfaz visual de Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapeamos los controladores para que los endpoints funcionen
app.MapControllers();

app.Run();