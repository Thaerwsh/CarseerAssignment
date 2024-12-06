namespace Application.Service.Base.Models;

using System.Text.Json.Serialization;

public class VehicleMake
{
    [JsonPropertyName("Make_ID")]
    public int MakeId { get; set; }

    [JsonPropertyName("Make_Name")]
    public string MakeName { get; set; }
}