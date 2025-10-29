using System.Text;

namespace Robin.Nodes;

public readonly struct BinaryOperatorNode(string @operator, IExpressionNode left, IExpressionNode right) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;

    public void Render(Context context, StringBuilder output)
    {
        throw new NotImplementedException();
    }
}


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

