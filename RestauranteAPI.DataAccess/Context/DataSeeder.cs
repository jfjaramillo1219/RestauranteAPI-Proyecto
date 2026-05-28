using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.DataAccess.Context;

public static class DataSeeder
{
    public static void SeedData(RestauranteDbContext context)
    {
        if (!context.Customers.Any())
        {
            var cliente1 = new Customer { FirstName = "Carlos", LastName = "Ramirez", Email = "carlos@test.com", Phone = "3001234567" };
            var cliente2 = new Customer { FirstName = "Diana", LastName = "Gomez", Email = "diana@test.com", Phone = "3109876543" };

            context.Customers.AddRange(cliente1, cliente2);
        }

        if (!context.Tables.Any())
        {
            var mesa1 = new RestaurantTable { Number = 1, Capacity = 2 };
            var mesa2 = new RestaurantTable { Number = 2, Capacity = 4 };
            var mesa3 = new RestaurantTable { Number = 3, Capacity = 6 };

            context.Tables.AddRange(mesa1, mesa2, mesa3);
        }

        if (!context.MenuItems.Any())
        {
            var plato1 = new MenuItem { Name = "Bandeja Paisa", Description = "Traditional dish", Price = 35000, IsAvailable = true };
            var plato2 = new MenuItem { Name = "Ajiaco", Description = "Traditional soup", Price = 28000, IsAvailable = true };
            var bebida1 = new MenuItem { Name = "Jugo de Mora", Description = "Natural juice", Price = 8000, IsAvailable = true };

            context.MenuItems.AddRange(plato1, plato2, bebida1);
        }

        context.SaveChanges();
    }
}
