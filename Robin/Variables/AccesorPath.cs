using System.Collections.Immutable;
using System.Text;

namespace Robin.Variables;

public readonly struct AccesorPath(ImmutableArray<IAccessor> segments)
{
    public static implicit operator AccesorPath(string value) => Parse(value);
    public static implicit operator string(AccesorPath value) => value.ToString();
    public ImmutableArray<IAccessor> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                return segment.Accept(InlineAccessorPrinter.Instance, sb);
            },
            sb => sb.ToString()
        );
    }

    public static AccesorPath Parse(string path)
    {
        if (string.IsNullOrEmpty(path))
            return new AccesorPath([]);

        var segments = new List<IAccessor>();
        int i = 0;

        while (i < path.Length)
        {
            if (path[i] == '.')
            {
                if (i == 0) segments.Add(new ThisAccessor());
                else i++; // skip '.'
            }
            else if (path[i] == '~')
            {
                if (i == 0) segments.Add(new ParentAccessor());
                else i++; // skip '.'
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
                    // Parse as chain path key
                    var chainPath = Parse(content);
                    segments.Add(new KeyAccessor(chainPath));
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

        return new AccesorPath([.. segments]);
    }
}
