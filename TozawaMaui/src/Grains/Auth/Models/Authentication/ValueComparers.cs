using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Grains.Auth.Models.Authentication;

internal class ListOfGuidsComparer : ValueComparer<List<Guid>>
{
    public ListOfGuidsComparer()
        : base((c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList())
    { }
}

internal class ListOfRolesComparer : ValueComparer<List<Role>>
{
    public ListOfRolesComparer()
        : base((c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList())
    { }
}

internal class DictionaryGuidStringComparer : ValueComparer<Dictionary<Guid, string>>
{
    public DictionaryGuidStringComparer()
        : base((c1, c2) => c1 == null && c2 == null || (c1 != null && c2 != null && c1.OrderBy(x => x.Key).SequenceEqual(c2.OrderBy(x => x.Key))),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c)
    { }
}


/* public static class ValueComparers
{
    public static ValueComparer<List<string>> ListStringValueComparer()
    {
        return new ValueComparer<List<string>>((c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
    }

    public static ValueComparer<List<Guid>> ListGuidValueComparer()
    {
        return new ValueComparer<List<Guid>>((c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
    }

    public static ValueComparer<Dictionary<Guid, List<Guid>>> DictionaryGuidListGuidValueComparer()
    {
        return new ValueComparer<Dictionary<Guid, List<Guid>>>((c1, c2) => c1 == null && c2 == null || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c);
    }

    public static ValueComparer<Dictionary<Guid, int>> DictionaryGuidIntValueComparer()
    {
        return new ValueComparer<Dictionary<Guid, int>>((c1, c2) => c1 == null && c2 == null || (c1 != null && c2 != null && c1.OrderBy(x => x.Key).SequenceEqual(c2.OrderBy(x => x.Key))),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c);
    }
    public static ValueComparer<Dictionary<Guid, string>> DictionaryGuidStringValueComparer()
    {
        return new ValueComparer<Dictionary<Guid, string>>((c1, c2) => c1 == null && c2 == null || (c1 != null && c2 != null && c1.OrderBy(x => x.Key).SequenceEqual(c2.OrderBy(x => x.Key))),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c);
    }
} */