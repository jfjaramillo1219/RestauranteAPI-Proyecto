using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestauranteAPI.API.Mappings;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.DataAccess.Repositories;
using RestauranteAPI.DataAccess.Seeders;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;
using RestauranteAPI.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<RestauranteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// 3. Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationMenuItemRepository, ReservationMenuItemRepository>();

// 4. Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationMenuItemService, ReservationMenuItemService>();

// 5. DataSeeder
builder.Services.AddScoped<DataSeeder>();

// 6. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurante API",
        Version = "v1",
        Description = "Sistema de Reservas de Restaurante — ITM 2026"
    });
});

// 7. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

var app = builder.Build();

// 8. Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestauranteDbContext>();
    db.Database.Migrate();
}

// 9. DataSeeder on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// 10. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
