namespace Api.Presentation.Controllers;

using Application.Service.Base.Interfaces;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VehicleTypesController : ControllerBase
{
    private readonly IVehicleTypeService _vehicleTypeService;

    public VehicleTypesController(IVehicleTypeService vehicleTypeService)
    {
        _vehicleTypeService = vehicleTypeService;
    }

    [HttpGet("{makeId}")]
    public async Task<IActionResult> GetVehicleTypesForMakeId(int makeId)
    {
        if (makeId <= 0)
            return BadRequest("Invalid make ID. Make ID must be greater than 0.");

        try
        {
            var vehicleTypesResult = await _vehicleTypeService.GetVehicleTypesForMakeIdAsync(makeId);

            if (!vehicleTypesResult.Success)
            {
                return StatusCode(500, $"An error occurred: {vehicleTypesResult.ErrorMessage}");
            }

            if (vehicleTypesResult.Data == null || !vehicleTypesResult.Data.Any())
            {
                return NotFound("No vehicle types found for the given make ID.");
            }

            return Ok(vehicleTypesResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}
