using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductPriceItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("purchasePrice")]
    public float PurchasePrice { get; set; }
    [JsonPropertyName("sellingPrice")]
    public float SellingPrice { get; set; }
    [JsonPropertyName("promoPrice")]
    public float PromoPrice { get; set; }

    [JsonPropertyName("startTime")]
    public string StartTime { get; set; }
    
    public OutputProductPriceItem(ProductPrice productPrice)
    {
        Id = productPrice.Id.ToString();
        PurchasePrice = productPrice.PurchasePrice;
        SellingPrice = productPrice.SellingPrice;
        PromoPrice = productPrice.PromoPrice;
        StartTime = productPrice.StartTime.ToString("O");
    }
}