using Application.Service.Base.Interfaces;
using Application.Service.Base.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Application.Service;

public class MakeService : IMakeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private const string CacheKey = "VehicleMakes";

    public MakeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _configuration = configuration;
    }

    public async Task<ApiResponseWrapper<PaginatedModels>> GetPaginatedMakesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var allMakes = await GetAllMakesAsync();
            int totalCount = allMakes.Count();

            var paginatedItems = allMakes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedModels = new PaginatedModels
            {
                Models = paginatedItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return new ApiResponseWrapper<PaginatedModels>
            {
                Success = true,
                Data = paginatedModels
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseWrapper<PaginatedModels>
            {
                Success = false,
                ErrorMessage = $"An error occurred while fetching paginated makes: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponseWrapper<PaginatedModels>> GetModelsAsync(int makeId, int year, string vehicleType, int pageNumber, int pageSize)
    {
        try
        {
            var cacheKey = $"Models_{makeId}_{year}_{vehicleType}";

            if (_memoryCache.TryGetValue(cacheKey, out List<VehicleMake> cachedVehicleMakes))
            {
                var paginatedModel = cachedVehicleMakes
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                return new ApiResponseWrapper<PaginatedModels>
                {
                    Success = true,
                    Data = new PaginatedModels
                    {
                        Models = paginatedModel,
                        TotalCount = cachedVehicleMakes.Count,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }

            string apiUrl = _configuration["VehicleModels"]
                .Replace("{makeId}", makeId.ToString())
                .Replace("{year}", year.ToString());

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return new ApiResponseWrapper<PaginatedModels>
                {
                    Success = false,
                    ErrorMessage = $"Error fetching data from API: {response.StatusCode}"
                };

            var responseData = await response.Content.ReadAsStringAsync();
            var models = JsonSerializer.Deserialize<MakeApiResponse>(responseData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (models == null || models.Results == null)
                return new ApiResponseWrapper<PaginatedModels>
                {
                    Success = false,
                    ErrorMessage = "Failed to fetch or deserialize models."
                };

            _memoryCache.Set(cacheKey, models.Results, TimeSpan.FromHours(1));

            var paginatedModels = models.Results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return new ApiResponseWrapper<PaginatedModels>
            {
                Success = true,
                Data = new PaginatedModels
                {
                    Models = paginatedModels,
                    TotalCount = models.Results.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseWrapper<PaginatedModels>
            {
                Success = false,
                ErrorMessage = $"An error occurred while fetching models: {ex.Message}"
            };
        }
    }

    private async Task<IEnumerable<VehicleMake>> GetAllMakesAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<VehicleMake> cachedMakes))
        {
            return cachedMakes;
        }

        string apiUrl = _configuration["VehicleMakes"];
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to fetch vehicle makes from external API.");
        }

        var responseData = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<MakeApiResponse>(responseData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var makes = apiResponse.Results.Select(result => new VehicleMake
        {
            MakeId = result.MakeId,
            MakeName = result.MakeName
        }).ToList();

        _memoryCache.Set(CacheKey, makes, TimeSpan.FromHours(6));

        return makes;
    }
}
