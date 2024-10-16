using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputUnitList
{
    [JsonPropertyName("units")]
    public ICollection<OutputUnitItem> Units { get; set; }

    public OutputUnitList(ICollection<Unit> units)
    {
        Units = units.Select(unit => new OutputUnitItem(unit)).ToArray();
    }
}