namespace EfiritPro.Retail.ProductModule.Models;

public class ProductGroup
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid OrganizationId { get; set; }
    
    public required string Name { get; set; }
    public Guid? ParentGroupId { get; set; }
    
    public virtual ProductGroup? ParentGroup { get; set; }
    public virtual List<ProductGroup> ChildGroups { get; set; } = new();
    public virtual List<Product> Products { get; set; } = new();
}