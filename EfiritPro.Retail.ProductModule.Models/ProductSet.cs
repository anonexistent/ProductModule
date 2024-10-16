namespace EfiritPro.Retail.ProductModule.Models
{
    public class ProductSet
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }
        public Guid OrganizationId { get; set; }

        public Guid ProductId { get; set; }
        public Guid IncludedProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Product IncludedProduct { get; set; }
    }
}
