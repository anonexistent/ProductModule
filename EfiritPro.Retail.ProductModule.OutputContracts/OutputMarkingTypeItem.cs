using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputMarkingTypeItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public OutputMarkingTypeItem(MarkingType productType)
    {
        Id = productType.Id.ToString();
        Name = productType.Name;
    }
}