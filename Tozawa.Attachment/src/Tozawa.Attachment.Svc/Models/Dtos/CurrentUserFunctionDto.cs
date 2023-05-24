using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.extension;

namespace Tozawa.Attachment.Svc.Models.Dtos;

public class CurrentUserFunctionDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public FunctionType FunctionType { get; set; }
    [JsonConverter(typeof(FunctionTypeConverter))]
    public FunctionType TextId { get; set; }
}