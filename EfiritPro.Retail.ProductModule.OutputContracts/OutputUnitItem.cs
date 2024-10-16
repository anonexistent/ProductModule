using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputUnitItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("code")]
    public ushort Code { get; set; }

    public OutputUnitItem(Unit unit)
    {
        Id = unit.Id.ToString();
        Name = unit.Name;
        Code = unit.Code;
    }
}