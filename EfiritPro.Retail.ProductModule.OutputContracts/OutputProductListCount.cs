using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputProductListCount
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    public OutputProductListCount(int count)
    {
        Count = count;
    }
}
