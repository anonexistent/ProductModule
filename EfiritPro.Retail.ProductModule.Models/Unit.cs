namespace EfiritPro.Retail.ProductModule.Models;

public class Unit
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public ushort Code { get; set; }

    public virtual List<Product> Products { get; set; } = new();
}