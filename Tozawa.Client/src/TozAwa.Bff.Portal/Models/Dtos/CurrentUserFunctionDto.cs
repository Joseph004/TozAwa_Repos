using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tozawa.Bff.Portal.extension;
using Tozawa.Bff.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Models.Dtos;

public class CurrentUserFunctionDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public FunctionType FunctionType { get; set; }
    [JsonConverter(typeof(FunctionTypeConverter))]
    public FunctionType TextId { get; set; }
}