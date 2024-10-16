using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.InputContracts
{
    public class CreateProductSetBody
    {
        [JsonPropertyName("productId")]
        public string productId { get; set; }
        [JsonPropertyName("includedProductsList")]
        public string includedProductId { get; set; }
    }
}
