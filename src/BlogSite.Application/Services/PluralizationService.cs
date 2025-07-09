using System.Text.RegularExpressions;

namespace BlogSite.Application.Services;

public interface IPluralizationService
{
    string Pluralize(string word);
    string Singularize(string word);
    bool IsPlural(string word);
}

public class PluralizationService : IPluralizationService
{
    private readonly Dictionary<string, string> _irregularPlurals = new()
    {
        { "person", "people" },
        { "child", "children" },
        { "foot", "feet" },
        { "tooth", "teeth" },
        { "mouse", "mice" },
        { "man", "men" },
        { "woman", "women" }
    };

    private readonly Dictionary<string, string> _irregularSingulars;

    public PluralizationService()
    {
        // Create reverse mapping for singularization
        _irregularSingulars = _irregularPlurals.ToDictionary(x => x.Value, x => x.Key);
    }

    public string Pluralize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        var lower = word.ToLowerInvariant();

        // Check irregular plurals first
        if (_irregularPlurals.TryGetValue(lower, out var irregular))
        {
            return PreserveCasing(word, irregular);
        }

        // Apply standard pluralization rules
        if (lower.EndsWith("y") && !IsVowel(lower[^2]))
        {
            return word[..^1] + "ies";
        }

        if (lower.EndsWith("s") || lower.EndsWith("x") || lower.EndsWith("z") || 
            lower.EndsWith("ch") || lower.EndsWith("sh"))
        {
            return word + "es";
        }

        if (lower.EndsWith("f"))
        {
            return word[..^1] + "ves";
        }

        if (lower.EndsWith("fe"))
        {
            return word[..^2] + "ves";
        }

        // Default: just add 's'
        return word + "s";
    }

    public string Singularize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        var lower = word.ToLowerInvariant();

        // Check irregular singulars first
        if (_irregularSingulars.TryGetValue(lower, out var irregular))
        {
            return PreserveCasing(word, irregular);
        }

        // Apply standard singularization rules
        if (lower.EndsWith("ies") && word.Length > 3)
        {
            return word[..^3] + "y";
        }

        if (lower.EndsWith("ves"))
        {
            if (word.Length > 3)
            {
                var stem = word[..^3];
                if (stem.ToLowerInvariant().EndsWith("l") || stem.ToLowerInvariant().EndsWith("r"))
                {
                    return stem + "f";
                }
                return stem + "fe";
            }
        }

        if (lower.EndsWith("ses") || lower.EndsWith("xes") || lower.EndsWith("zes") || 
            lower.EndsWith("ches") || lower.EndsWith("shes"))
        {
            return word[..^2];
        }

        if (lower.EndsWith("s") && word.Length > 1)
        {
            return word[..^1];
        }

        return word;
    }

    public bool IsPlural(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        var lower = word.ToLowerInvariant();

        // Check if it's an irregular plural
        if (_irregularSingulars.ContainsKey(lower))
            return true;

        // Check common plural endings
        return lower.EndsWith("s") || lower.EndsWith("ies") || lower.EndsWith("ves");
    }

    private static bool IsVowel(char c)
    {
        return "aeiou".Contains(char.ToLowerInvariant(c));
    }

    private static string PreserveCasing(string original, string replacement)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(replacement))
            return replacement;

        if (char.IsUpper(original[0]))
        {
            return char.ToUpperInvariant(replacement[0]) + replacement[1..].ToLowerInvariant();
        }

        return replacement.ToLowerInvariant();
    }
}