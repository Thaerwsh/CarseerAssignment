namespace Application.Service.Base.Interfaces;

using Application.Service.Base.Models;

public interface IMakeService
{
      Task<ApiResponseWrapper<PaginatedModels>> GetPaginatedMakesAsync(int pageNumber, int pageSize);
      Task<ApiResponseWrapper<PaginatedModels>> GetModelsAsync(int makeId, int year, string vehicleType, int pageNumber, int pageSize);
}