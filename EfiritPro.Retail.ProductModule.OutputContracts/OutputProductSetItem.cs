using EfiritPro.Retail.ProductModule.Models;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.OutputContracts
{
    public class OutputProductSetItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("organizationId")]
        public string OrgId { get; set; }
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }

        [JsonPropertyName("rootProduct")]
        public string RootProductId { get; set; }

        [JsonPropertyName("includedProduct")]
        public string IncludedProduct { get; set; }

        public OutputProductSetItem(ProductSet set)
        {
            Id = set.Id.ToString();
            OrgId=set.OrganizationId.ToString();
            OwnerId=set.OwnerId.ToString();
            RootProductId = set.ProductId.ToString();
            IncludedProduct = set.IncludedProductId.ToString();
        }
    }
}
