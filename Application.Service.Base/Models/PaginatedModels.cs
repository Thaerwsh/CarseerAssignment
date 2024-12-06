namespace Application.Service.Base.Models;

using System.Collections.Generic;

public class PaginatedModels
{
    public IEnumerable<VehicleMake> Models { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}