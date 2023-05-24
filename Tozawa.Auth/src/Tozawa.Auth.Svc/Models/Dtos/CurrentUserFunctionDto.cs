using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.Models.Dtos;

public class CurrentUserFunctionDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public FunctionType FunctionType { get; set; }
    [JsonConverter(typeof(FunctionTypeConverter))]
    public FunctionType TextId { get; set; }
}