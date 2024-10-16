using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductGroup
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("productGroups")]
    public ICollection<OutputProductGroup> ProductGroups { get; set; }
    [JsonPropertyName("products")]
    public ICollection<OutputProductItem> Products { get; set; } = Array.Empty<OutputProductItem>();
    [JsonPropertyName("parentGroupId")]
    public string? ParentGroupId { get; set; }

    public OutputProductGroup(ProductGroup group, bool withProducts)
    {
        Id = group.Id.ToString();
        Name = group.Name;
        ProductGroups = group.ChildGroups
            .Select(childGroup => new OutputProductGroup(childGroup, withProducts))
            .ToArray();
        if (withProducts)
        {
            Products = group.Products
                .Select(product => new OutputProductItem(product))
                .ToArray();
        }

        ParentGroupId = group.ParentGroupId?.ToString();
    }
}