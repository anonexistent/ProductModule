using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductPriceList
{
    [JsonPropertyName("productPrices")]
    public ICollection<OutputProductPriceItem> ProductPrices { get; set; }

    public OutputProductPriceList(ICollection<ProductPrice> productPrices)
    {
        ProductPrices = productPrices.Select(pp => new OutputProductPriceItem(pp)).ToArray();
    }
}