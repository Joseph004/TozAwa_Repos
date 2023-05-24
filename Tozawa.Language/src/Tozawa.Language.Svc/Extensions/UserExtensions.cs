using Newtonsoft.Json;
using Tozawa.Language.Svc.Helper;
using Tozawa.Language.Svc.Models.Enums;

namespace Tozawa.Language.Svc.extension
{
    public class FunctionTypeConverter : EnumToStringConverter<FunctionType>
    {
    public override bool CanWrite { get { return true; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, EnumHelper.GetDescription((FunctionType)value));
    }
}
public abstract class EnumToStringConverter<T> : JsonConverter where T : struct
{
    public override abstract bool CanWrite { get; }

    public override bool CanConvert(Type objectType) => objectType.IsEnum;
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    => EnumHelper.GetValueFromDescription<T>(reader.Value.ToString());
    public abstract override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
}

}