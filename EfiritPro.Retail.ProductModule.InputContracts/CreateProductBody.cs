using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public class CreateProductBody
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("vendorCode")]
    public required string VendorCode { get; set; }
    [JsonPropertyName("barCode")]
    public string? BarCode { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("price")]
    public CreateProductPriceBody? Price { get; set; }
    [JsonPropertyName("excise")]
    public required bool Excise { get; set; }
    
    [JsonPropertyName("productGroupId")]
    public string? ProductGroupId { get; set; }
    [JsonPropertyName("vatId")]
    public required string VATId { get; set; }
    [JsonPropertyName("productTypeId")]
    public required string ProductTypeId { get; set; }
    [JsonPropertyName("unitId")]
    public required string UnitId { get; set; }
    [JsonPropertyName("markingTypeId")]
    public string? MarkingTypeId { get; set; }
    
    [JsonPropertyName("hidden")]
    public required bool Hidden { get; set; }
    
    [JsonPropertyName("priceMayChangeInCashReceipt")]
    public bool PriceMayChangeInCashReceipt { get; set; }
    [JsonPropertyName("priceMinInCashReceipt")]
    public float PriceMinInCashReceipt { get; set; }
    [JsonPropertyName("priceMaxInCashReceipt")]
    public float PriceMaxInCashReceipt { get; set; }

    [JsonPropertyName("isAllowedToMove")]
    public bool IsAllowedToMove { get; set; }


    [JsonPropertyName("compositionList")] 
    public List<string> CompositionList { get; set; } = new();
}