

using System.ComponentModel;
using Microsoft.AspNetCore.JsonPatch;

namespace Tozawa.Auth.Svc.extension;

public static class JsonPatchDocumentExtensions
{
    public static bool IsUpdatedProperty(this JsonPatchDocument patchModel, string property)
    {
        return patchModel.Operations != null && patchModel.Operations.Any(x => string.Equals(x.path, property, StringComparison.CurrentCultureIgnoreCase));
    }

    public static bool IsAnyUpdatedProperty(this JsonPatchDocument patchModel, IEnumerable<string> properties)
    {
        return patchModel.Operations != null && patchModel.Operations.Any(x => properties.Contains(x.path));
    }

    public static bool IsOnlyUpdatedProperty(this JsonPatchDocument patchModel, string property)
    {
        return patchModel.IsUpdatedProperty(property) && patchModel.Operations.Count == 1;
    }

    public static T GetPatchValue<T>(this JsonPatchDocument patchModel, string property)
    {
        if (!patchModel.IsUpdatedProperty(property)) return default;
        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(patchModel.Operations.First(x => x.path == property).value.ToString());
    }
}