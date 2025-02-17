using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Grains.Auth.Models.Authentication;

internal class ListOfGuidsCoverter : ValueConverter<List<Guid>, string>
{
    public ListOfGuidsCoverter() :
        base(v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
             v => JsonConvert.DeserializeObject<List<Guid>>(v, new JsonSerializerSettings
             {
                 NullValueHandling = NullValueHandling.Ignore,
                 Formatting = Formatting.None
             }),
             null)
    { }
}
internal class ListOfRolesCoverter : ValueConverter<List<Role>, string>
{
    public ListOfRolesCoverter() :
        base(v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
             v => JsonConvert.DeserializeObject<List<Role>>(v, new JsonSerializerSettings
             {
                 NullValueHandling = NullValueHandling.Ignore,
                 Formatting = Formatting.None
             }),
             null)
    { }
}
internal class DictionaryGuidStringCoverter : ValueConverter<Dictionary<Guid, string>, string>
{
    public DictionaryGuidStringCoverter() :
        base(v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
             v => JsonConvert.DeserializeObject<Dictionary<Guid, string>>(v, new JsonSerializerSettings
             {
                 NullValueHandling = NullValueHandling.Ignore,
                 Formatting = Formatting.None
             }),
             null)
    { }
}