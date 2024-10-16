using EfiritPro.Retail.ProductModule.Models;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductListWithErrorList : OutputProductList
{
    [JsonPropertyName("errors")]
    public object Errors { get; set; }

    public OutputProductListWithErrorList(ICollection<Product> products, object errors) : base(products)
    {
        Errors = errors;
    }
}
