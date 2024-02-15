using System.Text.Json;
using System.Text.Json.Serialization;

namespace TozawaNGO;

public static class JsonFileUtils
{
    private static readonly JsonSerializerOptions _options =
        new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public static void SimpleWrite(object obj, string fileName)
    {
        var jsonString = JsonSerializer.Serialize(obj, _options);
        File.WriteAllText(fileName, jsonString);
    }
}