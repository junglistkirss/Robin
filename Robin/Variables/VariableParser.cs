using Robin.Contracts;
using Robin.Contracts.Variables;
using Robin.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Variables;

public static class VariableParser
{
    //public static bool TryParseVariable(this ReadOnlySpan<char> path, [NotNullWhen(true)] out VariablePath? accesorPath)
    //{
    //    try
    //    {
    //        accesorPath = ParseVariable(path);
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        accesorPath = null;
    //        return false;
    //    }
    //}

    public static VariablePath ParseVariable(this ref ExpressionLexer lexer, Extract path)
    {
        //if (path is Range.Empty)
        //    return new VariablePath([]);

        List<IVariableSegment> segments = [];
        int i = path.Start;
        int end = path.End;

        while (i < end)
        {
            if (lexer[i] == '.')
            {
                if (i == path.Start)
                    segments.Add(ThisSegment.Instance);
                i++; // skip '.'
            }
            else if (lexer[i] == '[')
            {
                i++; // skip '['

                // Skip leading whitespace
                while (i < end && char.IsWhiteSpace(lexer[i]))
                    i++;

                // Could be numeric index or chain path key
                int start = i;
                int bracketDepth = 1;

                // Find the matching closing bracket
                while (i < end && bracketDepth > 0)
                {
                    if (lexer[i] == '[')
                        bracketDepth++;
                    else if (lexer[i] == ']')
                        bracketDepth--;

                    if (bracketDepth > 0)
                        i++;
                }

                if (bracketDepth != 0)
                    throw new FormatException("Unclosed accessor");

                //ReadOnlySpan<char> content = lexer[start..i].Trim();

                //// Try to parse as numeric index first
                //if (int.TryParse(content, out int index))
                //{
                    segments.Add(new IndexSegment(new Extract(start, i)));
                //}
                //else
                //{
                //    throw new FormatException($"Invalid index accessor: {content}");
                //}

                i++; // skip ']'
                // Don't continue here - let the loop naturally handle what comes next
            }
            // Parse member accessor
            else
            {
                int start = i;
                while (i < end && lexer[i] != '.' && lexer[i] != '[')
                    i++;
                
                if (start == i)
                    throw new FormatException("Empty member name");

                segments.Add(new MemberSegment(new Extract(start, i)));
            }
        }

        return new VariablePath([.. segments]);
    }
}
