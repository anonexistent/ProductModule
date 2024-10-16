namespace EfiritPro.Retail.ProductModule.Models;

public class FavoriteProduct
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid OrganizationId { get; set; }
    
    public Guid? WorkerId { get; set; }
    
    public virtual Product Product { get; set; }
}