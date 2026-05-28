using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.API.DTOs.Table;

public class UpdateTableDto
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public TableStatus Status { get; set; }
}
