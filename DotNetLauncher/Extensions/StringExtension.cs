using System.Text.RegularExpressions;

namespace DotNetLauncher.Extensions;

public static partial class StringExtension
{
    [GeneratedRegex("[^a-zA-Z]", RegexOptions.Compiled)]
    private static partial Regex NonLettersRegex();

    [GeneratedRegex("^[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex TwoOrMoreLettersRegex();

    public static bool ShouldBeDisabled(this string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return true;

        word = word.TrimNonLetters();

        return !word.IsSingleWord();
    }

    public static string TrimNonLetters(this string word) => NonLettersRegex().Replace(word, string.Empty);

    private static bool IsSingleWord(this string word) =>
        word.Trim().Split(' ').Length == 1 && TwoOrMoreLettersRegex().IsMatch(word);
}