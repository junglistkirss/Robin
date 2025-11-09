using System.Buffers;

namespace RobinMustache.Internals;

public static class HtmlEscaper
{

    public static string? Escape(this string? value)
    {
        if (value is null || string.IsNullOrEmpty(value) )
            return null;

        var pool = ArrayPool<char>.Shared;
        char[] buffer = pool.Rent(value.Length * 6); // worst case, every char escaped
        int pos = 0;

        try
        {
            foreach (char c in value)
            {
                switch (c)
                {
                    case '&': pos += "&amp;".CopyTo(buffer, pos); break;
                    case '<': pos += "&lt;".CopyTo(buffer, pos); break;
                    case '>': pos += "&gt;".CopyTo(buffer, pos); break;
                    case '"': pos += "&quot;".CopyTo(buffer, pos); break;
                    case '\'': pos += "&#39;".CopyTo(buffer, pos); break;
                    case '`': pos += "&#96;".CopyTo(buffer, pos); break;
                    case '=': pos += "&#61;".CopyTo(buffer, pos); break;
                    default: buffer[pos++] = c; break;
                }
            }

            return new string(buffer, 0, pos);
        }
        finally
        {
            pool.Return(buffer);
        }
    }
    private static int CopyTo(this string s, char[] buffer, int index)
    {
        s.CopyTo(0, buffer, index, s.Length);
        return s.Length;
    }
}
