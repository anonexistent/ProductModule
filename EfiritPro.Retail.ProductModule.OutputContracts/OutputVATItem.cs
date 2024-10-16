using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputVATItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("percent")]
    public ushort Percent { get; set; }

    public OutputVATItem(VAT vat)
    {
        Id = vat.Id.ToString();
        Name = vat.Name;
        Percent = vat.Percent;
    }
}