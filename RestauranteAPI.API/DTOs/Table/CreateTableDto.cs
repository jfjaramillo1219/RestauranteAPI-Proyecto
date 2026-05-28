namespace RestauranteAPI.API.DTOs.Table;

public class CreateTableDto
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
}
