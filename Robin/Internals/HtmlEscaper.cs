namespace Robin.Internals;

public static class HtmlEscaper
{
    /// <summary>
    /// Échappe les caractères HTML essentiels dans la chaîne fournie.
    /// Retourne la même instance de chaîne si rien n'est à échapper.
    /// </summary>
    public static string? Escape(this string? value)
    {
        if (value is null) return null;

        int len = value.Length;
        int extra = 0; // longueur supplémentaire nécessaire

        // Comptage des occurrences pour calculer la nouvelle longueur exacte
        for (int i = 0; i < len; i++)
        {
            switch (value[i])
            {
                case '&': extra += 4; break; // & -> &amp; (5 chars, +4)
                case '<': extra += 3; break; // < -> &lt;  (4 chars, +3)
                case '>': extra += 3; break; // > -> &gt;  (4 chars, +3)
                case '"': extra += 5; break; // " -> &quot; (6 chars, +5)
                case '\'': extra += 4; break; // ' -> &#39;  (5 chars, +4)
                case '`': extra += 4; break; // ' -> &#96;  (5 chars, +4)
                case '=': extra += 4; break; // ' -> &#61;  (5 chars, +4)
                default: break;
            }
        }

        // Rien à faire -> retourner l'instance d'origine (optimisation importante)
        if (extra == 0) return value;

        int newLen = len + extra;

        // Construire la nouvelle chaîne en une seule allocation
        return string.Create(newLen, value, (span, src) =>
        {
            int pos = 0;
            ReadOnlySpan<char> amp = "&amp;";
            ReadOnlySpan<char> lt = "&lt;";
            ReadOnlySpan<char> gt = "&gt;";
            ReadOnlySpan<char> quot = "&quot;";
            ReadOnlySpan<char> apos = "&#39;";
            ReadOnlySpan<char> btck = "&#96;";
            ReadOnlySpan<char> eq = "&#61;";

            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];
                switch (c)
                {
                    case '&':
                        amp.CopyTo(span.Slice(pos));
                        pos += amp.Length;
                        break;
                    case '<':
                        lt.CopyTo(span.Slice(pos));
                        pos += lt.Length;
                        break;
                    case '>':
                        gt.CopyTo(span.Slice(pos));
                        pos += gt.Length;
                        break;
                    case '"':
                        quot.CopyTo(span.Slice(pos));
                        pos += quot.Length;
                        break;
                    case '\'':
                        apos.CopyTo(span.Slice(pos));
                        pos += apos.Length;
                        break;
                    case '`':
                        btck.CopyTo(span.Slice(pos));
                        pos += btck.Length;
                        break;
                    case '=':
                        eq.CopyTo(span.Slice(pos));
                        pos += eq.Length;
                        break;
                    default:
                        span[pos++] = c;
                        break;
                }
            }
        });
    }
}
