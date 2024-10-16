using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputVATList
{
    [JsonPropertyName("vats")]
    public ICollection<OutputVATItem> VATs { get; set; }

    public OutputVATList(ICollection<VAT> vats)
    {
        VATs = vats.Select(vat => new OutputVATItem(vat)).ToArray();
    }
}