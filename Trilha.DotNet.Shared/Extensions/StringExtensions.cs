namespace Trilha.DotNet.Shared.Extensions;

public static class StringExtensions
{
    public static string PadLeftZero(this int valor, int casas) => valor.ToString().PadLeft(casas, '0');

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public static string WithoutDiacritics(this string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        string[] diacritics =
        ["ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û"];
        string[] withoutDiacritics = ["c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U"];

        for (var i = 0; i < diacritics.Length; i++)
        {
            text = text.Replace(diacritics[i], withoutDiacritics[i]);
        }

        string[] caracteresEspeciais = ["¹", "²", "³", "£", "¢", "¬", "º", "¨", "\"", "'", ".", ",", "-", ":", "(", ")", "ª", "|", @"\\", "°", "_", "@", "#", "!", "$", "%", "&", "*", ";", "/", "<", ">", "?", "[", "]", "{", "}", "=", "+", "§", "´", "`", "^", "~"];

        text = caracteresEspeciais.Aggregate(text, (current, t) => current.Replace(t, string.Empty));

        text = Regex.Replace(text, @"[^\w\.@-]", " ",
            RegexOptions.None, TimeSpan.FromSeconds(1.5));

        return text.Trim();
    }

    public static string WithoutWhiteSpace(this string input)
    {
        int j = 0, inputlen = input.Length;
        var newarr = new char[inputlen];

        for (var i = 0; i < inputlen; ++i)
        {
            var tmp = input[i];

            if (char.IsWhiteSpace(tmp)) continue;
            newarr[j] = tmp;
            ++j;
        }

        return new string(newarr, 0, j);
    }

    public static string Coalesce(this string[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
    }

    public static string? NullIfEmpty(this string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;

    public static Stream ToStream(this string s, Encoding? encoding = null)
        => new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(s));

    public static string AsString(this Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDecimal(out var number) => number.ToString(CultureInfo.InvariantCulture),
            JsonTokenType.False => "false",
            JsonTokenType.True => "true",
            JsonTokenType.String => reader.TryGetString(),
            _ => null
        } ?? string.Empty;
    }

    public static string? TryGetString(this Utf8JsonReader reader)
    {
        try
        {
            return reader.GetString();
        }
        catch
        {
            return null;
        }
    }
}