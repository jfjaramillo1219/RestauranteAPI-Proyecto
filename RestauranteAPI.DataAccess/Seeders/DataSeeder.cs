using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.DataAccess.Seeders;

public class DataSeeder
{
    private readonly RestauranteDbContext _context;

    public DataSeeder(RestauranteDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await SeedCustomersAsync();
        await SeedTablesAsync();
        await SeedMenuItemsAsync();
        await SeedReservationsAsync();
        await SeedReservationMenuItemsAsync();
    }

    private async Task SeedCustomersAsync()
    {
        if (await _context.Customers.AnyAsync()) return;
        var customers = new List<Customer>
        {
            new() { FirstName="Carlos Andrés", LastName="Gómez Restrepo",    Email="carlos.gomez@email.com",      Phone="3001234567" },
            new() { FirstName="María Paula",   LastName="Hernández Vélez",   Email="maria.hernandez@email.com",   Phone="3112345678" },
            new() { FirstName="Juan David",    LastName="Martínez López",    Email="juan.martinez@email.com",     Phone="3223456789" },
            new() { FirstName="Ana Sofía",     LastName="Rodríguez Castaño", Email="ana.rodriguez@email.com",     Phone="3334567890" },
            new() { FirstName="Luis Fernando", LastName="Pérez Moreno",      Email="luis.perez@email.com",        Phone="3445678901" },
            new() { FirstName="Valentina",     LastName="Torres Quintero",   Email="valentina.torres@email.com",  Phone="3556789012" },
            new() { FirstName="Sebastián",     LastName="Ramírez Osorio",    Email="sebastian.ramirez@email.com", Phone="3667890123" },
            new() { FirstName="Daniela",       LastName="Vargas Giraldo",    Email="daniela.vargas@email.com",    Phone="3778901234" },
            new() { FirstName="Andrés Felipe", LastName="Castro Salazar",    Email="andres.castro@email.com",     Phone="3889012345" },
            new() { FirstName="Camila Andrea", LastName="Díaz Muñoz",        Email="camila.diaz@email.com",       Phone="3990123456" }
        };
        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTablesAsync()
    {
        if (await _context.Tables.AnyAsync()) return;
        var tables = new List<RestaurantTable>
        {
            new() { Number=1, Capacity=2,  Location="Íntimo",          Status=TableStatus.Available },
            new() { Number=2, Capacity=2,  Location="Íntimo",          Status=TableStatus.Available },
            new() { Number=3, Capacity=4,  Location="Salón Principal", Status=TableStatus.Available },
            new() { Number=4, Capacity=4,  Location="Salón Principal", Status=TableStatus.Reserved  },
            new() { Number=5, Capacity=4,  Location="Salón Principal", Status=TableStatus.Available },
            new() { Number=6, Capacity=6,  Location="Terraza",         Status=TableStatus.Available },
            new() { Number=7, Capacity=6,  Location="Terraza",         Status=TableStatus.Reserved  },
            new() { Number=8, Capacity=10, Location="Salón VIP",       Status=TableStatus.Available }
        };
        await _context.Tables.AddRangeAsync(tables);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMenuItemsAsync()
    {
        if (await _context.MenuItems.AnyAsync()) return;
        var items = new List<MenuItem>
        {
            // Appetizers
            new() { Name="Patacones con hogao",          Description="Patacones crujientes con hogao casero",       Price=12000, Category=MenuItemCategory.Appetizer,  IsAvailable=true },
            new() { Name="Empanadas de pipián (3 und.)", Description="Empanadas fritas rellenas de pipián",         Price=9000,  Category=MenuItemCategory.Appetizer,  IsAvailable=true },
            new() { Name="Arepa de chócolo con quesito", Description="Arepa dulce de chócolo con queso campesino",  Price=8000,  Category=MenuItemCategory.Appetizer,  IsAvailable=true },
            // Main courses
            new() { Name="Bandeja paisa completa",       Description="Frijoles, arroz, chicharrón, huevo, chorizo", Price=38000, Category=MenuItemCategory.MainCourse, IsAvailable=true },
            new() { Name="Sancocho de gallina",          Description="Sancocho tradicional con gallina criolla",     Price=28000, Category=MenuItemCategory.MainCourse, IsAvailable=true },
            new() { Name="Ajiaco santafereño",           Description="Ajiaco con pollo, papas y guascas",           Price=25000, Category=MenuItemCategory.MainCourse, IsAvailable=true },
            new() { Name="Cazuela de mariscos",          Description="Mariscos frescos en salsa criolla",            Price=35000, Category=MenuItemCategory.MainCourse, IsAvailable=true },
            // Desserts
            new() { Name="Arroz con leche",              Description="Arroz con leche con canela y pasas",           Price=8000,  Category=MenuItemCategory.Dessert,    IsAvailable=true },
            new() { Name="Tres leches artesanal",        Description="Bizcocho empapado en tres leches",             Price=12000, Category=MenuItemCategory.Dessert,    IsAvailable=true },
            new() { Name="Postre de natas",              Description="Postre tradicional antioqueño",                Price=9000,  Category=MenuItemCategory.Dessert,    IsAvailable=true },
            // Beverages
            new() { Name="Limonada de coco",             Description="Limonada cremosa con coco rallado",            Price=8000,  Category=MenuItemCategory.Beverage,   IsAvailable=true },
            new() { Name="Jugo natural de lulo",         Description="Jugo de lulo natural sin azúcar",              Price=7000,  Category=MenuItemCategory.Beverage,   IsAvailable=true },
            new() { Name="Agua panela con limón",        Description="Agua panela fría con limón y hielo",           Price=5000,  Category=MenuItemCategory.Beverage,   IsAvailable=true }
        };
        await _context.MenuItems.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReservationsAsync()
    {
        if (await _context.Reservations.AnyAsync()) return;

        var customers = await _context.Customers.ToListAsync();
        var tables = await _context.Tables.ToListAsync();

        var today = DateTime.Today;

        var reservations = new List<Reservation>
        {
            // Confirmed (tables 4 and 7 are Reserved — match these)
            new() { CustomerId=customers[0].Id, TableId=tables[3].Id, ReservationDate=today.AddDays(1),  PartySize=3, Status=ReservationStatus.Confirmed, Notes="Cumpleaños" },
            new() { CustomerId=customers[1].Id, TableId=tables[6].Id, ReservationDate=today.AddDays(2),  PartySize=5, Status=ReservationStatus.Confirmed, Notes="Aniversario" },
            new() { CustomerId=customers[2].Id, TableId=tables[2].Id, ReservationDate=today.AddDays(3),  PartySize=4, Status=ReservationStatus.Confirmed, Notes=null },
            // Completed
            new() { CustomerId=customers[3].Id, TableId=tables[0].Id, ReservationDate=today.AddDays(-3), PartySize=2, Status=ReservationStatus.Completed, Notes=null },
            new() { CustomerId=customers[4].Id, TableId=tables[4].Id, ReservationDate=today.AddDays(-5), PartySize=3, Status=ReservationStatus.Completed, Notes="Mesa especial" },
            new() { CustomerId=customers[5].Id, TableId=tables[1].Id, ReservationDate=today.AddDays(-7), PartySize=2, Status=ReservationStatus.Completed, Notes=null },
            // Cancelled
            new() { CustomerId=customers[6].Id, TableId=tables[5].Id, ReservationDate=today.AddDays(-2), PartySize=4, Status=ReservationStatus.Cancelled, Notes="Cancelado por cliente" },
            new() { CustomerId=customers[7].Id, TableId=tables[7].Id, ReservationDate=today.AddDays(-4), PartySize=6, Status=ReservationStatus.Cancelled, Notes=null },
            // Pending
            new() { CustomerId=customers[8].Id, TableId=tables[2].Id, ReservationDate=today.AddDays(5),  PartySize=3, Status=ReservationStatus.Pending,   Notes=null },
            new() { CustomerId=customers[9].Id, TableId=tables[5].Id, ReservationDate=today.AddDays(6),  PartySize=5, Status=ReservationStatus.Pending,   Notes="Sin gluten" },
            new() { CustomerId=customers[0].Id, TableId=tables[0].Id, ReservationDate=today.AddDays(7),  PartySize=1, Status=ReservationStatus.Pending,   Notes=null },
            new() { CustomerId=customers[1].Id, TableId=tables[7].Id, ReservationDate=today.AddDays(8),  PartySize=8, Status=ReservationStatus.Pending,   Notes="Reunión de negocios" },
            new() { CustomerId=customers[2].Id, TableId=tables[1].Id, ReservationDate=today.AddDays(9),  PartySize=2, Status=ReservationStatus.Pending,   Notes=null },
            new() { CustomerId=customers[3].Id, TableId=tables[4].Id, ReservationDate=today.AddDays(10), PartySize=4, Status=ReservationStatus.Pending,   Notes=null },
            new() { CustomerId=customers[4].Id, TableId=tables[6].Id, ReservationDate=today.AddDays(11), PartySize=6, Status=ReservationStatus.Pending,   Notes="Terraza preferida" }
        };
        await _context.Reservations.AddRangeAsync(reservations);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReservationMenuItemsAsync()
    {
        if (await _context.ReservationMenuItems.AnyAsync()) return;

        var reservations = await _context.Reservations.ToListAsync();
        var menuItems = await _context.MenuItems.ToListAsync();

        // Add items only to Confirmed (indexes 0,1,2) and Completed (indexes 3,4,5)
        var entries = new List<ReservationMenuItem>
        {
            // Confirmed reservation 0: appetizer + main + beverage
            new() { ReservationId=reservations[0].Id, MenuItemId=menuItems[0].Id,  Quantity=2 },
            new() { ReservationId=reservations[0].Id, MenuItemId=menuItems[3].Id,  Quantity=3 },
            new() { ReservationId=reservations[0].Id, MenuItemId=menuItems[10].Id, Quantity=3 },
            // Confirmed reservation 1: appetizer + main + dessert + beverage
            new() { ReservationId=reservations[1].Id, MenuItemId=menuItems[1].Id,  Quantity=3 },
            new() { ReservationId=reservations[1].Id, MenuItemId=menuItems[5].Id,  Quantity=5 },
            new() { ReservationId=reservations[1].Id, MenuItemId=menuItems[8].Id,  Quantity=2 },
            new() { ReservationId=reservations[1].Id, MenuItemId=menuItems[11].Id, Quantity=5 },
            // Confirmed reservation 2: main + beverage
            new() { ReservationId=reservations[2].Id, MenuItemId=menuItems[4].Id,  Quantity=4 },
            new() { ReservationId=reservations[2].Id, MenuItemId=menuItems[12].Id, Quantity=4 },
            // Completed reservation 3: appetizer + main + dessert
            new() { ReservationId=reservations[3].Id, MenuItemId=menuItems[2].Id,  Quantity=1 },
            new() { ReservationId=reservations[3].Id, MenuItemId=menuItems[6].Id,  Quantity=2 },
            new() { ReservationId=reservations[3].Id, MenuItemId=menuItems[7].Id,  Quantity=2 },
            // Completed reservation 4: main + dessert + beverage
            new() { ReservationId=reservations[4].Id, MenuItemId=menuItems[3].Id,  Quantity=3 },
            new() { ReservationId=reservations[4].Id, MenuItemId=menuItems[9].Id,  Quantity=3 },
            new() { ReservationId=reservations[4].Id, MenuItemId=menuItems[11].Id, Quantity=3 },
            // Completed reservation 5: appetizer + main + beverage
            new() { ReservationId=reservations[5].Id, MenuItemId=menuItems[0].Id,  Quantity=1 },
            new() { ReservationId=reservations[5].Id, MenuItemId=menuItems[4].Id,  Quantity=2 },
            new() { ReservationId=reservations[5].Id, MenuItemId=menuItems[10].Id, Quantity=2 }
        };
        await _context.ReservationMenuItems.AddRangeAsync(entries);
        await _context.SaveChangesAsync();
    }
}
