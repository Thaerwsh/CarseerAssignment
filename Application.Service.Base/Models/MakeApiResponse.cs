namespace Application.Service.Base.Models;

public class MakeApiResponse
{
    public int Count { get; set; }
    public string Message { get; set; }
    public string SearchCriteria { get; set; }
    public List<VehicleMake> Results { get; set; }
}
