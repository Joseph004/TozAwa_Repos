using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;

namespace Tozawa.Language.Svc.Models.Dtos;

public class CurrentUserFunctionDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public FunctionType FunctionType { get; set; }
    [JsonConverter(typeof(FunctionTypeConverter))]
    public FunctionType TextId { get; set; }
}