using Robin.Nodes;
using Robin.Nodes.Expressions;
using System.Globalization;

namespace Robin.tests;

public class ExpressionParserTests
{
    [Fact]
    public void EmptyExpression()
    {
        ReadOnlySpan<char> source = "".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();

        Assert.Null(node);
    }


    [Theory]
    [InlineData("two")]
    [InlineData("one")]
    [InlineData("one.two")]
    [InlineData("one[0].two")]
    [InlineData("one[0][1].two")]
    [InlineData("one[test].two")]
    [InlineData("one[test[3]].two")]
    [InlineData("one[test.one.two[3]].two")]
    public void IdenitiferExpression(string ident)
    {
        ReadOnlySpan<char> source = ident.AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        IdentifierNode operand = Assert.IsType<IdentifierNode>(node);
        Assert.Equal(ident, operand.Path);
    }
    [Theory]
    [InlineData("+", "two")]
    [InlineData("-", "one")]
    public void UnaryExpression(string op, string ident)
    {
        ReadOnlySpan<char> source = $"{op}{ident}".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        UnaryOperationNode unary = Assert.IsType<UnaryOperationNode>(node);
        Assert.Equal(op, unary.Operator);
        IdentifierNode operand = Assert.IsType<IdentifierNode>(unary.Operand);
        Assert.Equal(ident, operand.Path);
    }

    [Theory]
    [InlineData("+", "two")]
    [InlineData("-", "one")]
    public void UnaryFunctionExpression(string op, string funcName)
    {
        ReadOnlySpan<char> source = $"{op}{funcName}()".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        UnaryOperationNode unary = Assert.IsType<UnaryOperationNode>(node);
        Assert.Equal(op, unary.Operator);
        FunctionCallNode operand = Assert.IsType<FunctionCallNode>(unary.Operand);
        Assert.Equal(funcName, operand.FunctionName);
        Assert.Empty(operand.Arguments);
    }

    [Theory]
    [InlineData("+", "321")]
    [InlineData("-", "123")]
    [InlineData("-", "0.5")]
    public void UnaryConstantExpression(string op, string member)
    {
        ReadOnlySpan<char> source = $"{op}{member}".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        UnaryOperationNode unary = Assert.IsType<UnaryOperationNode>(node);
        Assert.Equal(op, unary.Operator);
        NumberNode operand = Assert.IsType<NumberNode>(unary.Operand);
        Assert.Equal(double.Parse(member, CultureInfo.InvariantCulture), operand.Constant);
    }


    [Fact]
    public void FunctionNoArgsExpression()
    {
        ReadOnlySpan<char> source = "func()".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Empty(func.Arguments);
    }

    [Theory]
    [InlineData("one", "+", "two")]
    [InlineData("one", "-", "two")]
    [InlineData("one", "/", "two")]
    [InlineData("one", "*", "two")]
    [InlineData("one", "^", "two")]
    [InlineData("one", "%", "two")]
    public void OperatorFunctionNoArgsExpression(string funcLeft, string funcOp, string funcRight)
    {
        ReadOnlySpan<char> source = $"{funcLeft}() {funcOp} {funcRight}()".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        BinaryOperationNode func = Assert.IsType<BinaryOperationNode>(node);
        Assert.Equal(funcOp, func.Operator);
        FunctionCallNode left = Assert.IsType<FunctionCallNode>(func.Left);
        Assert.Equal(funcLeft, left.FunctionName);
        Assert.Empty(left.Arguments);
        FunctionCallNode right = Assert.IsType<FunctionCallNode>(func.Right);
        Assert.Equal(funcRight, right.FunctionName);
        Assert.Empty(right.Arguments);
    }
    [Theory]
    [InlineData("func", "test")]
    [InlineData("func", "test[0]")]
    [InlineData("func", "test[a.b.c]")]
    public void FunctionExpression(string funcName, string ident)
    {
        ReadOnlySpan<char> source = $"{funcName}({ident})".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal(funcName, func.FunctionName);
        IExpressionNode arg = Assert.Single(func.Arguments);
        IdentifierNode identifier = Assert.IsType<IdentifierNode>(arg);
        Assert.Equal(ident, identifier.Path);
    }

    [Fact]
    public void FunctionNestedExpression()
    {
        ReadOnlySpan<char> source = "func(one nested(two))".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierNode ident = Assert.IsType<IdentifierNode>(func.Arguments[0]);
        Assert.Equal("one", ident.Path);
        FunctionCallNode nested = Assert.IsType<FunctionCallNode>(func.Arguments[1]);
        Assert.Equal("nested", nested.FunctionName);
        IExpressionNode nestedArg = Assert.Single(nested.Arguments);
        IdentifierNode nestedIdent = Assert.IsType<IdentifierNode>(nestedArg);
        Assert.Equal("two", nestedIdent.Path);
    }

    [Fact]
    public void FunctionManyArgs()
    {
        ReadOnlySpan<char> source = "func(one two)".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierNode ident1 = Assert.IsType<IdentifierNode>(func.Arguments[0]);
        Assert.Equal("one", ident1.Path);
        IdentifierNode ident2 = Assert.IsType<IdentifierNode>(func.Arguments[1]);
        Assert.Equal("two", ident2.Path);
    }

    [Theory]
    [InlineData("one", "+", "two")]
    [InlineData("one", "-", "two")]
    [InlineData("one", "/", "two")]
    [InlineData("one", "*", "two")]
    [InlineData("one", "^", "two")]
    [InlineData("one", "%", "two")]
    public void OperatorManyArgs(string left, string op, string right)
    {
        ReadOnlySpan<char> source = $"{left} {op} {right}".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        BinaryOperationNode func = Assert.IsType<BinaryOperationNode>(node);
        Assert.Equal(op, func.Operator);
        IdentifierNode ident1 = Assert.IsType<IdentifierNode>(func.Left);
        Assert.Equal(left, ident1.Path);
        IdentifierNode ident2 = Assert.IsType<IdentifierNode>(func.Right);
        Assert.Equal(right, ident2.Path);
    }

    [Theory]
    [InlineData("one", "+", "two", "*", "three")]
    [InlineData("one", "-", "two", "*", "three")]
    [InlineData("one", "/", "two", "*", "three")]
    [InlineData("one", "*", "two", "*", "three")]
    [InlineData("one", "^", "two", "*", "three")]
    [InlineData("one", "%", "two", "*", "three")]
    [InlineData("one", "+", "two", "/", "three")]
    [InlineData("one", "-", "two", "/", "three")]
    [InlineData("one", "/", "two", "/", "three")]
    [InlineData("one", "*", "two", "/", "three")]
    [InlineData("one", "^", "two", "/", "three")]
    [InlineData("one", "%", "two", "/", "three")]
    [InlineData("one", "+", "two", "+", "three")]
    [InlineData("one", "-", "two", "+", "three")]
    [InlineData("one", "/", "two", "+", "three")]
    [InlineData("one", "*", "two", "+", "three")]
    [InlineData("one", "^", "two", "+", "three")]
    [InlineData("one", "%", "two", "+", "three")]
    [InlineData("one", "+", "two", "-", "three")]
    [InlineData("one", "-", "two", "-", "three")]
    [InlineData("one", "/", "two", "-", "three")]
    [InlineData("one", "*", "two", "-", "three")]
    [InlineData("one", "^", "two", "-", "three")]
    [InlineData("one", "%", "two", "-", "three")]
    [InlineData("one", "+", "two", "%", "three")]
    [InlineData("one", "-", "two", "%", "three")]
    [InlineData("one", "/", "two", "%", "three")]
    [InlineData("one", "*", "two", "%", "three")]
    [InlineData("one", "^", "two", "%", "three")]
    [InlineData("one", "%", "two", "%", "three")]
    public void OperatorsManyArgs(string left, string firstOp, string innerLeft, string innerOp, string innerRight)
    {
        ReadOnlySpan<char> source = $"{left} {firstOp} ({innerLeft} {innerOp} {innerRight})".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        BinaryOperationNode op = Assert.IsType<BinaryOperationNode>(node);
        Assert.Equal(firstOp, op.Operator);
        IdentifierNode ident1 = Assert.IsType<IdentifierNode>(op.Left);
        Assert.Equal(left, ident1.Path);
        BinaryOperationNode op2 = Assert.IsType<BinaryOperationNode>(op.Right);
        Assert.Equal(innerOp, op2.Operator);
        IdentifierNode ident2 = Assert.IsType<IdentifierNode>(op2.Left);
        Assert.Equal(innerLeft, ident2.Path);
        IdentifierNode ident3 = Assert.IsType<IdentifierNode>(op2.Right);
        Assert.Equal(innerRight, ident3.Path);
    }

    [Fact]
    public void FunctionManyArgs__Malformed()
    {
        ReadOnlySpan<char> source = "func(one two".AsSpan();
        ExpressionLexer lexer = new(source);
        try
        {
            IExpressionNode? nodes = lexer.Parse();
        }
        catch (Exception)
        {
            Assert.True(true, "Expected exception thrown");
        }
    }
}
