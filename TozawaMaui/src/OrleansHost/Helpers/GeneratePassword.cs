namespace OrleansHost.Helpers;

public static class GeneratePassword
{
    private static Random rand = new();
    public static string RandomPassword(int length = 8)
    {
        string[] categories = {
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
        "abcdefghijklmnopqrstuvwxyz",
        "!-_*+&$",
        "0123456789" };

        List<char> chars = new(length);

        // add one char from each category
        foreach (string cat in categories)
        {
            chars.Add(cat[rand.Next(cat.Length)]);
        }

        // add random chars from any category until we hit the length
        string all = string.Concat(categories);
        while (chars.Count < length)
        {
            chars.Add(all[rand.Next(all.Length)]);
        }

        // shuffle and return our password
        var shuffled = chars.OrderBy(c => rand.NextDouble()).ToArray();
        return new string(shuffled);
    }
}