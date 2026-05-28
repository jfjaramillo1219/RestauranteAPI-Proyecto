using AutoMapper;
using RestauranteAPI.API.DTOs.Customer;
using RestauranteAPI.API.DTOs.MenuItem;
using RestauranteAPI.API.DTOs.Reservation;
using RestauranteAPI.API.DTOs.ReservationMenuItem;
using RestauranteAPI.API.DTOs.Table;
using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<Customer, CustomerDto>();

        // RestaurantTable
        CreateMap<CreateTableDto, RestaurantTable>();
        CreateMap<UpdateTableDto, RestaurantTable>();
        CreateMap<RestaurantTable, TableDto>()
            .ForMember(d => d.Status,
                o => o.MapFrom(s => s.Status.ToString()));

        // MenuItem
        CreateMap<CreateMenuItemDto, MenuItem>();
        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(d => d.Category,
                o => o.MapFrom(s => s.Category.ToString()));

        // Reservation
        CreateMap<CreateReservationDto, Reservation>();
        CreateMap<UpdateReservationDto, Reservation>();
        CreateMap<Reservation, ReservationDto>()
            .ForMember(d => d.CustomerName,
                o => o.MapFrom(s => s.Customer.FirstName + " " + s.Customer.LastName))
            .ForMember(d => d.TableNumber,
                o => o.MapFrom(s => s.Table.Number))
            .ForMember(d => d.TableCapacity,
                o => o.MapFrom(s => s.Table.Capacity))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<Reservation, ReservationDetailDto>()
            .IncludeBase<Reservation, ReservationDto>()
            .ForMember(d => d.Items,
                o => o.MapFrom(s => s.ReservationMenuItems))
            .ForMember(d => d.TotalAmount,
                o => o.MapFrom(s => s.ReservationMenuItems
                    .Sum(ri => ri.MenuItem.Price * ri.Quantity)));

        // ReservationMenuItem
        CreateMap<AddMenuItemDto, ReservationMenuItem>();
        CreateMap<ReservationMenuItem, ReservationMenuItemDto>()
            .ForMember(d => d.MenuItemName,
                o => o.MapFrom(s => s.MenuItem.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.MenuItem.Price))
            .ForMember(d => d.Subtotal,
                o => o.MapFrom(s => s.MenuItem.Price * s.Quantity));
    }
}
