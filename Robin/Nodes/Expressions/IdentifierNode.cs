using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes.Expressions;

public interface IPathSegment;
public readonly struct ChainPath(ImmutableArray<IPathSegment> segments)
{
    public static implicit operator ChainPath(string value) => Parse(value);
    public static implicit operator string(ChainPath value) => value.ToString();
    public ImmutableArray<IPathSegment> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                switch (segment)
                {
                    case MemberAccesor member:
                        if (sb.Length > 0)
                            sb.Append('.');
                        sb.Append(member.MemberName);
                        break;
                    case IndexAccessor index:
                        sb.Append('[').Append(index.Index).Append(']');
                        break;
                    case KeyAccessor chainKey:
                        sb.Append('[').Append(chainKey.Key.ToString()).Append(']');
                        break;
                }
                return sb;
            },
            sb => sb.ToString()
        );
    }

    public static ChainPath Parse(string path)
    {
        if (string.IsNullOrEmpty(path))
            return new ChainPath([]);

        var segments = new List<IPathSegment>();
        int i = 0;

        while (i < path.Length)
        {
            // Skip dots
            if (path[i] == '.')
            {
                i++;
                continue;
            }

            // Parse bracket accessor (index, static key, or chain path key)
            if (path[i] == '[')
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

                segments.Add(new MemberAccesor(memberName));
            }
        }

        return new ChainPath([.. segments]);
    }
}

public readonly struct MemberAccesor(string memberName) : IPathSegment
{
    public string MemberName { get; } = memberName;
}
public readonly struct IndexAccessor(int index) : IPathSegment
{
    public int Index { get; } = index;
}

public readonly struct KeyAccessor(ChainPath key) : IPathSegment
{
    public ChainPath Key { get; } = key;
}
public readonly struct IdentifierNode(ChainPath path) : IExpressionNode
{
    public ChainPath Path { get; } = path;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIdenitifer(this, args);
    }
};


//// Example usage and testing
//public class Program
//{
//    public static void Main()
//    {
//        TestExpression("3 + 4 * 2");
//        TestExpression("(3 + 4) * 2");
//        TestExpression("foo(1, 2, 3)");
//        TestExpression("max(a, b) + min(c, d)");
//        TestExpression("-5 + 3");
//        TestExpression("a > 5 && b < 10");
//        TestExpression("x == 5 || y == 10");
//        TestExpression("2 ^ 3 ^ 2");
//        TestExpression("price * 1.2");
//        TestExpression("obj.property[0]");
//    }

//    static void TestExpression(string expr)
//    {
//        Console.WriteLine($"\nParsing: {expr}");
//        try
//        {
//            var parser = new ExpressionParser(expr.AsSpan());
//            var result = parser.Parse();
//            Console.WriteLine($"Result: {result}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}

