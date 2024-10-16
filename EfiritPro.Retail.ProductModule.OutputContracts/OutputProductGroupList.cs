using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductGroupList
{
    [JsonPropertyName("productGroups")]
    public ICollection<OutputProductGroup> ProductGroups { get; set; }

    public OutputProductGroupList(ICollection<ProductGroup> productGroups, bool withProducts)
    {
        ProductGroups = productGroups
            .Select(pg => new OutputProductGroup(pg, withProducts))
            .ToArray();
    }
}