using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputFavoriteProductList
{
    [JsonPropertyName("favoriteProducts")]
    public string[] FavoriteProducts { get; set; }

    public OutputFavoriteProductList(ICollection<FavoriteProduct> favoriteProducts)
    {
        FavoriteProducts = favoriteProducts
            .Select(fp => fp.ProductId.ToString())
            .ToArray();
    }
}