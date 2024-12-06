namespace Application.Service.Base.Models;

public class VehicleTypeApiResponse
{
    public List<VehicleTypeResult> Results { get; set; }

    public class VehicleTypeResult
    {
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }
    }
}
