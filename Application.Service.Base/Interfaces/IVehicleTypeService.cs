namespace Application.Service.Base.Interfaces;

using Application.Service.Base.Models;

public interface IVehicleTypeService
{
    Task<ApiResponseWrapper<List<VehicleType>>> GetVehicleTypesForMakeIdAsync(int makeId);
}