using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record TargetGroupQuery : OwnerAndOrganizationIdQuery
{
    [JsonPropertyName("newGroupId")]
    public string? NewGroupId { get; set; }
}
