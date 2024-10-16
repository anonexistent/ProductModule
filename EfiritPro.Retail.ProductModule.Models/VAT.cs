namespace EfiritPro.Retail.ProductModule.Models;

public class VAT
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public ushort Percent { get; set; }

    public virtual List<Product> Products { get; set; } = new();
}