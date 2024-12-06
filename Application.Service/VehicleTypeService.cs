namespace Application.Service;

using Application.Service.Base.Interfaces;
using Application.Service.Base.Models;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using System.Text.Json;

public class VehicleTypeService : IVehicleTypeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    private const string CacheKeyPrefix = "VehicleTypesForMake_";

    public VehicleTypeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _configuration = configuration;
    }

    public async Task<ApiResponseWrapper<List<VehicleType>>> GetVehicleTypesForMakeIdAsync(int makeId)
    {
        var cacheKey = $"{CacheKeyPrefix}{makeId}";

        try
        {
            if (_memoryCache.TryGetValue(cacheKey, out List<VehicleType> cachedVehicleTypes))
            {
                return new ApiResponseWrapper<List<VehicleType>>
                {
                    Success = true,
                    Data = cachedVehicleTypes
                };
            }

            var apiUrl = _configuration["VehicleTypes"]?.Replace("{makeId}", makeId.ToString());

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponseWrapper<List<VehicleType>>
                {
                    Success = false,
                    ErrorMessage = $"Failed to fetch vehicle types from external API. Status Code: {response.StatusCode}"
                };
            }

            var responseData = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<VehicleTypeApiResponse>(responseData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Results == null)
            {
                return new ApiResponseWrapper<List<VehicleType>>
                {
                    Success = false,
                    ErrorMessage = "No results found in the API response."
                };
            }

            var vehicleTypes = apiResponse.Results.Select(result => new VehicleType
            {
                VehicleTypeId = result.VehicleTypeId,
                VehicleTypeName = result.VehicleTypeName
            }).ToList();

            _memoryCache.Set(cacheKey, vehicleTypes, TimeSpan.FromHours(6));

            return new ApiResponseWrapper<List<VehicleType>>
            {
                Success = true,
                Data = vehicleTypes
            };
        }
        catch (Exception)
        {
            return new ApiResponseWrapper<List<VehicleType>>
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred while processing the vehicle types. Please try again later."
            };
        }
    }
}
