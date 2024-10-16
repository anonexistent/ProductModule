using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("vendorCode")]
    public string VendorCode { get; set; }
    [JsonPropertyName("barCode")]
    public string? BarCode { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("excise")]
    public bool Excise { get; set; }
    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }
    
    [JsonPropertyName("priceMayChangeInCashReceipt")]
    public bool PriceMayChangeInCashReceipt { get; set; }
    [JsonPropertyName("priceMinInCashReceipt")]
    public float PriceMinInCashReceipt { get; set; }
    [JsonPropertyName("priceMaxInCashReceipt")]
    public float PriceMaxInCashReceipt { get; set; }
    
    [JsonPropertyName("compositionList")]
    public List<string> CompositionList { get; set; }
    
    [JsonPropertyName("productGroupId")]
    public string? ProductGroupId { get; set; }
    [JsonPropertyName("vat")]
    public OutputVATItem VAT { get; set; }
    [JsonPropertyName("productType")]
    public OutputProductTypeItem ProductType { get; set; }
    [JsonPropertyName("unit")]
    public OutputUnitItem Unit { get; set; }
    [JsonPropertyName("markingType")]
    public OutputMarkingTypeItem? MarkingType { get; set; }
    
    [JsonPropertyName("purchasePrice")]
    public float PurchasePrice { get; set; }
    [JsonPropertyName("sellingPrice")]
    public float SellingPrice { get; set; }
    [JsonPropertyName("promoPrice")]
    public float PromoPrice { get; set; }

    [JsonPropertyName("isAllowedToMove")]
    public bool IsAllowedToMove { get; set; }

    [JsonPropertyName("includedProducts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<OutputProductItem> IncludedItems { get; set; } = Array.Empty<OutputProductItem>();
    //[JsonPropertyName("parentProductId")]
    //public OutputProductItem? ParentProductId { get; set; }

    public OutputProductItem(Product product)
    {
        Id = product.Id.ToString();

        Name = product.Name;
        VendorCode = product.VendorCode;
        BarCode = product.BarCode;
        Description = product.Description;
        Excise = product.Excise;
        Hidden = product.Hidden;

        PriceMayChangeInCashReceipt = product.PriceMayChangeInCashReceipt;
        PriceMinInCashReceipt = product.PriceMinInCashReceipt;
        PriceMaxInCashReceipt = product.PriceMaxInCashReceipt;

        CompositionList = product.CompositionList.Select(c => c.ToString()).ToList();

        ProductGroupId = product.ProductGroupId?.ToString();
        VAT = new OutputVATItem(product.VAT);
        ProductType = new OutputProductTypeItem(product.ProductType);
        Unit = new OutputUnitItem(product.Unit);
        MarkingType = product.MarkingType is not null ? new OutputMarkingTypeItem(product.MarkingType) : null;

        PurchasePrice = product.PurchasePrice;
        SellingPrice = product.SellingPrice;
        PromoPrice = product.PromoPrice;

        IsAllowedToMove = product.IsAllowedToMove;

        var tempInProducts = new List<OutputProductItem>();

        foreach (var item in product.includingProducts)
        {
            tempInProducts.Add(new OutputProductItem(item.IncludedProduct));

            Console.WriteLine(item.Product.Name);
        }

        //IncludedItems = product.includingProducts.Select(x=>new OutputProductItem(x.Product)).ToArray();

        if (tempInProducts.Count > 0) IncludedItems = tempInProducts;
        else IncludedItems = null;

        //ParentProductId = new OutputProductItem(product.ParentProduct) is not null ? new OutputProductItem(product.ParentProduct) : null;
    }
}