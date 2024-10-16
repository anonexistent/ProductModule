namespace EfiritPro.Retail.ProductModule.Models;

public class ProductPrice
{
    public Guid Id { get; set; }
    
    public Guid OwnerId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProductId { get; set; }

    public Guid? CreateByPostingId { get; set; } = null; 
    
    public DateTime StartTime { get; set; }
    
    public float PurchasePrice { get; set; }
    public float SellingPrice { get; set; }
    public float PromoPrice { get; set; }
    
    public virtual Product Product { get; set; }
}