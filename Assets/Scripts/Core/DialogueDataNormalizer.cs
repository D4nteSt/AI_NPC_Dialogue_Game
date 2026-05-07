using System;

public static class DialogueDataNormalizer
{
    public static string NormalizeId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim().ToLowerInvariant();
    }

    public static string NormalizeText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim();
    }

    public static bool IdEquals(string a, string b)
    {
        return string.Equals(
            NormalizeId(a),
            NormalizeId(b),
            StringComparison.OrdinalIgnoreCase
        );
    }
}