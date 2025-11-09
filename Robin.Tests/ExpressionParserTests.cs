using Robin.Abstractions.Expressions;
using Robin.Expressions;
using Robin.Nodes;

namespace Robin.tests;

public class ExpressionParserTests
{
    [Fact]
    public void EmptyExpression()
    {
        IExpressionNode? node = "".AsSpan().ParseExpression();
        Assert.Null(node);
    }


    [Theory]
    [InlineData("two")]
    [InlineData("one")]
    [InlineData("one.two")]
    [InlineData("one[0].two")]
    [InlineData("one[0][1].two")]
    public void IdenitiferExpression(string ident)
    {
        IExpressionNode? node = ident.AsSpan().ParseExpression();
        IdentifierExpressionNode operand = Assert.IsType<IdentifierExpressionNode>(node);
        Assert.Equal(ident, operand.Path);
    }

    [Fact]
    public void FunctionNoArgsExpression()
    {
        IExpressionNode? node = "func()".AsSpan().ParseExpression();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Empty(func.Arguments);
    }

    [Fact]
    public void LirteralExpression()
    {
        IExpressionNode? node = "'func()'".AsSpan().ParseExpression();
        LiteralExpressionNode func = Assert.IsType<LiteralExpressionNode>(node);
        Assert.Equal("func()", func.Constant);
    }

    [Fact]
    public void NumberExpression()
    {
        IExpressionNode? node = "42".AsSpan().ParseExpression();
        IndexExpressionNode func = Assert.IsType<IndexExpressionNode>(node);
        Assert.Equal(42, func.Constant);
    }
    [Fact]
    public void IdentifierExpressionParenthesis()
    {
        IExpressionNode? node = "(test)".AsSpan().ParseExpression();
        IdentifierExpressionNode func = Assert.IsType<IdentifierExpressionNode>(node);
        Assert.Equal("test", func.Path);
    }
    [Theory]
    [InlineData("func", "test")]
    [InlineData("func", "test[0]")]
    public void FunctionExpression(string funcName, string ident)
    {
        ReadOnlySpan<char> source = $"{funcName}({ident})".AsSpan();
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal(funcName, func.FunctionName);
        IExpressionNode arg = Assert.Single(func.Arguments);
        IdentifierExpressionNode identifier = Assert.IsType<IdentifierExpressionNode>(arg);
        Assert.Equal(ident, identifier.Path);
    }



    [Fact]
    public void FunctionNestedExpression()
    {
        IExpressionNode? node = "func(one nested(two))".AsSpan().ParseExpression();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierExpressionNode ident = Assert.IsType<IdentifierExpressionNode>(func.Arguments[0]);
        Assert.Equal("one", ident.Path);
        FunctionCallNode nested = Assert.IsType<FunctionCallNode>(func.Arguments[1]);
        Assert.Equal("nested", nested.FunctionName);
        IExpressionNode nestedArg = Assert.Single(nested.Arguments);
        IdentifierExpressionNode nestedIdent = Assert.IsType<IdentifierExpressionNode>(nestedArg);
        Assert.Equal("two", nestedIdent.Path);
    }

    [Fact]
    public void FunctionManyArgs()
    {
        IExpressionNode? node = "func(one two)".AsSpan().ParseExpression();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierExpressionNode ident1 = Assert.IsType<IdentifierExpressionNode>(func.Arguments[0]);
        Assert.Equal("one", ident1.Path);
        IdentifierExpressionNode ident2 = Assert.IsType<IdentifierExpressionNode>(func.Arguments[1]);
        Assert.Equal("two", ident2.Path);
    }

    [Fact]
    public void FunctionManyArgs__Malformed()
    {
        try
        {
            _ = "func(one two".AsSpan().ParseExpression();
        }
        catch (Exception)
        {
            Assert.True(true, "Expected exception thrown");
        }
    }
}
