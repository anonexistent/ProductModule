using System.Text.Json.Serialization;
using EfiritPro.Retail.ProductModule.Models;

namespace EfiritPro.Retail.ProductModule.OutputContracts;

public class OutputMarkingTypeList
{
    [JsonPropertyName("markingTypes")]
    public ICollection<OutputMarkingTypeItem> Types { get; set; }

    public OutputMarkingTypeList(ICollection<MarkingType> types)
    {
        Types = types.Select(type => new OutputMarkingTypeItem(type)).ToArray();
    }
}