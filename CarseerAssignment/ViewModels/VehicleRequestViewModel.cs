namespace Carseer.ViewModels;

public class VehicleRequestViewModel
{
    public int MakeId { get; set; }
    public int Year { get; set; }
    public string VehicleType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
