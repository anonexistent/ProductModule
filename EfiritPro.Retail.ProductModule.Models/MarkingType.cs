namespace EfiritPro.Retail.ProductModule.Models;

public class MarkingType
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public virtual List<Product> Products { get; set; } = new();
}