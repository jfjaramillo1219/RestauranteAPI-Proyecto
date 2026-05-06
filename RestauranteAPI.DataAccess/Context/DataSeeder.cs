using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.DataAccess.Context
{
    public static class DataSeeder
    {
        public static void SeedData(RestauranteDbContext context)
        {
            // Check if DB already has customers
            if (!context.Customers.Any())
            {
                var cliente1 = new Customer { FirstName = "Carlos", LastName = "Ramirez", Email = "carlos@test.com", Phone = "3001234567" };
                var cliente2 = new Customer { FirstName = "Diana", LastName = "Gomez", Email = "diana@test.com", Phone = "3109876543" };

                context.Customers.AddRange(cliente1, cliente2);
            }

            // Check if DB already has tables
            if (!context.Tables.Any())
            {
                var mesa1 = new Table { TableNumber = 1, Capacity = 2, IsAvailable = true };
                var mesa2 = new Table { TableNumber = 2, Capacity = 4, IsAvailable = true };
                var mesa3 = new Table { TableNumber = 3, Capacity = 6, IsAvailable = true };

                context.Tables.AddRange(mesa1, mesa2, mesa3);
            }

            // Check if DB already has menu items
            if (!context.MenuItems.Any())
            {
                var plato1 = new MenuItem { Name = "Bandeja Paisa", Description = "Traditional dish", Price = 35000, IsAvailable = true };
                var plato2 = new MenuItem { Name = "Ajiaco", Description = "Traditional soup", Price = 28000, IsAvailable = true };
                var bebida1 = new MenuItem { Name = "Jugo de Mora", Description = "Natural juice", Price = 8000, IsAvailable = true };

                context.MenuItems.AddRange(plato1, plato2, bebida1);
            }

            // Save all generated data to the database
            context.SaveChanges();
        }
    }
}