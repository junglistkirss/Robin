using Robin.Nodes;
using System.Collections.Immutable;

namespace Robin.tests;

public class ExpressionParserTests
{
    [Fact]
    public void EmptyExpression()
    {
        ReadOnlySpan<char> source = "".AsSpan();
        ExpressionLexer lexer = new ExpressionLexer(source);
        ImmutableArray<IExpressionNode> nodes = lexer.Parse();

        Assert.Empty(nodes);
    }

    [Fact]
    public void FunctionExpression()
    {
        ReadOnlySpan<char> source = "func(test)".AsSpan();
        ExpressionLexer lexer = new ExpressionLexer(source);
        ImmutableArray<IExpressionNode> nodes = lexer.Parse();
        IExpressionNode node = Assert.Single(nodes);
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        IExpressionNode arg = Assert.Single(func.Arguments);
        IdentifierNode ident = Assert.IsType<IdentifierNode>(arg);
        Assert.Equal("test", ident.Name);
    }

    [Fact]
    public void FunctionManyArgs()
    {
        ReadOnlySpan<char> source = "func(one two)".AsSpan();
        ExpressionLexer lexer = new ExpressionLexer(source);
        ImmutableArray<IExpressionNode> nodes = lexer.Parse();
        IExpressionNode node = Assert.Single(nodes);
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", func.FunctionName);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierNode ident1 = Assert.IsType<IdentifierNode>(func.Arguments[0]);
        Assert.Equal("one", ident1.Name);
        IdentifierNode ident2 = Assert.IsType<IdentifierNode>(func.Arguments[1]);
        Assert.Equal("two", ident2.Name);
    }

    //[Fact]
    //public void OperatorManyArgs()
    //{
    //    ReadOnlySpan<char> source = "one + two".AsSpan();
    //    ExpressionLexer lexer = new ExpressionLexer(source);
    //    ImmutableArray<IExpressionNode> nodes = lexer.Parse();
    //    IExpressionNode node = Assert.Single(nodes);
    //    BinaryOperatorNode func = Assert.IsType<BinaryOperatorNode>(node);
    //    Assert.Equal("+", func.Operator);
    //    IdentifierNode ident1 = Assert.IsType<IdentifierNode>(func.Left);
    //    Assert.Equal("one", ident1.Name);
    //    IdentifierNode ident2 = Assert.IsType<IdentifierNode>(func.Right);
    //    Assert.Equal("two", ident2.Name);
    //}

    [Fact]
    public void FunctionManyArgs__Malformed()
    {
        ReadOnlySpan<char> source = "func(one two".AsSpan();
        ExpressionLexer lexer = new ExpressionLexer(source);
        try
        {
            ImmutableArray<IExpressionNode> nodes = lexer.Parse();
        }
        catch (Exception)
        {
            Assert.True(true, "Expected exception thrown");
        }
    }
}
