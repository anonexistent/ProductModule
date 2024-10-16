using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductTypeList
{
    [JsonPropertyName("productTypes")]
    public ICollection<OutputProductTypeItem> Types { get; set; }

    public OutputProductTypeList(ICollection<ProductType> types)
    {
        Types = types.Select(type => new OutputProductTypeItem(type)).ToArray();
    }
}