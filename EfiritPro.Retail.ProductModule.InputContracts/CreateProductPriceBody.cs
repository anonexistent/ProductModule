using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public class CreateProductPriceBody
{
    [JsonPropertyName("purchasePrice")]
    public required float PurchasePrice { get; set; }
    [JsonPropertyName("sellingPrice")]
    public required float SellingPrice { get; set; }
    [JsonPropertyName("promoPrice")]
    public required float PromoPrice { get; set; }
    
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
}