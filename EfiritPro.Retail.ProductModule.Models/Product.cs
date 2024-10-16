namespace EfiritPro.Retail.ProductModule.Models;

public class Product
{
    public Guid Id { get; set; }
    
    public Guid OwnerId { get; set; }
    public Guid OrganizationId { get; set; }
    
    public string Name { get; set; }
    public string VendorCode { get; set; }
    public string? BarCode { get; set; }
    public string? Description { get; set; }
    public bool Excise { get; set; }
    public bool Hidden { get; set; }
    
    public float PurchasePrice { get; set; }
    public float SellingPrice { get; set; }
    public float PromoPrice { get; set; }
    
    public Guid? ProductGroupId { get; set; }
    public Guid VATId { get; set; }
    public Guid ProductTypeId { get; set; }
    public Guid UnitId { get; set; }
    public Guid? MarkingTypeId { get; set; }
    public DateTime? PriceShouldBeSetInTime { get; set; }
    
    public bool PriceMayChangeInCashReceipt { get; set; }
    public float PriceMinInCashReceipt { get; set; }
    public float PriceMaxInCashReceipt { get; set; }

    public bool IsAllowedToMove { get; set; }

    public List<Guid> CompositionList { get; set; } = new();

    public virtual ICollection<ProductSet> includingProducts { get; set; } = new List<ProductSet>();
    public virtual ICollection<ProductSet> includedProducts { get; set; } = new List<ProductSet>();

    public virtual ProductGroup? ProductGroup { get; set; }
    public virtual VAT VAT { get; set; }
    public virtual ProductType ProductType { get; set; }
    public virtual Unit Unit { get; set; }
    public virtual MarkingType? MarkingType { get; set; }

    public virtual List<ProductPrice> ProductPrices { get; set; } = new();
}