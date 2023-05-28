

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Tozawa.Client.Portal.HttpClients;

public abstract class PatchBase
{
    public List<string> RemoveRequest { get; set; } = new List<string>();
    public virtual JsonPatchDocument ToPatchDocument()
    {
        var propertiesWithValues = GetType().GetProperties()
            .Where(x => !string.Equals(x.Name, nameof(RemoveRequest)))
            .Select(pi => new { Val = pi.GetValue(this), pi.Name })
            .Where(pi => pi.Val != null)
            .ToDictionary(pi => pi.Name, pi => pi.Val);

        var doc = new JsonPatchDocument();
        doc.Operations.AddRange(propertiesWithValues.Select(x => new Operation
        {
            path = x.Key,
            op = "replace",
            value = x.Value
        }));
        if (RemoveRequest.Any())
        {
            doc.Operations.AddRange(RemoveRequest.Select(x => new Operation
            {
                path = x,
                op = "remove"
            }));

        }

        return doc;
    }
}

public static class PatchExtensions
{
    public static void AddNullableProperty<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyExpression, object newValue, object oldValue) where TSource : PatchBase
    {
        if (propertyExpression.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression {propertyExpression} refers to a method, not a property.");
        }

        PropertyInfo? propInfo = member.Member as PropertyInfo;

        if (propInfo == null)
        {
            throw new ArgumentException($"Expression {propertyExpression} refers to a field, not a property.");
        }
        if (newValue == null && oldValue != null)
        {
            source.RemoveRequest.Add(propInfo.Name);
            return;
        }
        propInfo.SetValue(source, newValue);
    }
}
