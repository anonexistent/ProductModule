using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductList
{
    [JsonPropertyName("products")]
    public ICollection<OutputProductItem> Products { get; set; }

    public OutputProductList(ICollection<Product> products)
    {
        Products = products.Select(product => new OutputProductItem(product)).ToArray();
    }
}