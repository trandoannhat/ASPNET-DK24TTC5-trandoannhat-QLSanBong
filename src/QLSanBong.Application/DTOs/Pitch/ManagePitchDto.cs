namespace QLSanBong.Application.DTOs.Pitch;

public class CreatePitchDto
{
    public string Name { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdatePitchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }
}