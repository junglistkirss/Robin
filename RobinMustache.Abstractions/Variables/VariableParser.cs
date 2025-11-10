namespace RobinMustache.Abstractions.Variables;

public static class VariableParser
{
    public static bool TryParse(this string path, out VariablePath? accesorPath)
    {
        try
        {
            accesorPath = path.Parse();
            return true;
        }
        catch (Exception)
        {
            accesorPath = null;
            return false;
        }
    }

    public static VariablePath Parse(this string strPath)
    {
        if (string.IsNullOrEmpty(strPath))
            return new VariablePath([]);

        ReadOnlySpan<char> path = strPath.AsSpan();

        List<IVariableSegment> segments = [];
        int i = 0;

        while (i < path.Length)
        {
            if (path[i] == '.')
            {
                if (i == 0)
                    segments.Add(ThisSegment.Instance);
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

                string content = path.Slice(start, i - start).Trim().ToString();

                // Try to parse as numeric index first
                if (int.TryParse(content.ToString(), out int index))
                {
                    segments.Add(new IndexSegment(index));
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

                string memberName = path.Slice(start, i - start).ToString();
                if (string.IsNullOrEmpty(memberName))
                    throw new FormatException("Empty member name");

                segments.Add(new MemberSegment(memberName));
            }
        }

        return new VariablePath([.. segments]);
    }
}
