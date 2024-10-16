using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductTypeItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public OutputProductTypeItem(ProductType productType)
    {
        Id = productType.Id.ToString();
        Name = productType.Name;
    }
}