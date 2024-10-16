using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public class CreateProductGroupBody
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("parentGroupId")]
    public string? ParentGroupId { get; set; }
}