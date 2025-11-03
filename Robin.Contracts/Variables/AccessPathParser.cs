using System.Diagnostics.CodeAnalysis;

namespace Robin.Contracts.Variables;

public static class AccessPathParser
{
    public static bool TryParse(this string path, [NotNullWhen(true)] out VariablePath? accesorPath)
    {
        try
        {
            accesorPath = Parse(path);
            return true;
        }
        catch (Exception)
        {
            accesorPath = null;
            return false;
        }
    }

    public static VariablePath Parse(this string path)
    {
        if (string.IsNullOrEmpty(path))
            return new VariablePath([]);

        List<IAccessor> segments = [];
        int i = 0;

        while (i < path.Length)
        {
            if (path[i] == '.')
            {
                if (i == 0)
                    segments.Add(ThisAccessor.Instance);
                i++; // skip '.'
            }
            else if (path[i] == '[')
            {
                i++; // skip '['

                // Skip leading whitespace
                while (i < path.Length && char.IsWhiteSpace(path[i]))
                    i++;

                // Could be numeric index or chain path key
                int start = i;
                int bracketDepth = 1;

                // Find the matching closing bracket
                while (i < path.Length && bracketDepth > 0)
                {
                    if (path[i] == '[')
                        bracketDepth++;
                    else if (path[i] == ']')
                        bracketDepth--;

                    if (bracketDepth > 0)
                        i++;
                }

                if (bracketDepth != 0)
                    throw new FormatException("Unclosed accessor");

                string content = path[start..i].Trim();

                // Try to parse as numeric index first
                if (int.TryParse(content, out int index))
                {
                    segments.Add(new IndexAccessor(index));
                }
                else
                {
                    throw new FormatException($"Invalid index accessor: {content}");
                }

                i++; // skip ']'
                // Don't continue here - let the loop naturally handle what comes next
            }
            // Parse member accessor
            else
            {
                int start = i;
                while (i < path.Length && path[i] != '.' && path[i] != '[')
                    i++;

                string memberName = path[start..i];
                if (string.IsNullOrEmpty(memberName))
                    throw new FormatException("Empty member name");

                segments.Add(new MemberAccessor(memberName));
            }
        }

        return new VariablePath([.. segments]);
    }
}
