using Application.Service.Base.Interfaces;
using Carseer.ViewModels;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MakesController : ControllerBase
{
    private readonly IMakeService _makeService;

    public MakesController(IMakeService makeService)
    {
        _makeService = makeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedMakes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Page number and page size must be greater than 0.");

        try
        {
            var paginatedMakesResult = await _makeService.GetPaginatedMakesAsync(pageNumber, pageSize);

            if (!paginatedMakesResult.Success)
            {
                return StatusCode(500, $"An error occurred: {paginatedMakesResult.ErrorMessage}");
            }

            if (paginatedMakesResult.Data == null || paginatedMakesResult.Data == null)
            {
                return NotFound("No makes found for the given criteria.");
            }

            return Ok(paginatedMakesResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost("search")]
    public async Task<IActionResult> GetModels([FromBody] VehicleRequestViewModel request)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
            return BadRequest("Page number and page size must be greater than 0.");

        if (string.IsNullOrWhiteSpace(request.VehicleType))
            return BadRequest("Vehicle type cannot be empty.");

        try
        {
            var modelsResult = await _makeService.GetModelsAsync(request.MakeId, request.Year, request.VehicleType, request.PageNumber, request.PageSize);

            if (!modelsResult.Success || !string.IsNullOrWhiteSpace(modelsResult.ErrorMessage))
            {
                return StatusCode(500, $"An error occurred: {modelsResult.ErrorMessage}");
            }

            if (modelsResult.Data == null || modelsResult.Data == null)
                return NotFound("No models found for the given criteria.");

            return Ok(modelsResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
