using Robin.Contracts;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Expressions;
using Robin.Nodes;
using Robin.Variables;
using System;

namespace Robin.tests;

public class ExpressionParserTests
{
    [Fact]
    public void EmptyExpression()
    {
        ReadOnlySpan<char> source = "".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Empty);
        IExpressionNode? node = eLexer.Parse();
        Assert.Null(node);
    }


    [Theory]
    [InlineData("two")]
    [InlineData("one")]
    [InlineData("one  ")]
    [InlineData("  one")]
    [InlineData("  one  ")]
    public void IdenitiferExpression(string ident)
    {
        ReadOnlySpan<char> source = ident.AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        IdentifierExpressionNode id = Assert.IsType<IdentifierExpressionNode>(node);
        IVariableSegment first = Assert.Single(id.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal(ident.Trim(), source[(Range)member.MemberName]);
    }

    [Fact]
    public void FunctionNoArgsExpression()
    {
        ReadOnlySpan<char> source = "func()".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", source[(Range)func.FunctionName]);
        Assert.Empty(func.Arguments);
    }

    [Fact]
    public void LirteralExpression()
    {
        ReadOnlySpan<char> source = "'func()'".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        LiteralExpressionNode func = Assert.IsType<LiteralExpressionNode>(node);
        Assert.Equal("func()", source[(Range)func.Constant]);
    }

    [Fact]
    public void NumberExpression()
    {
        ReadOnlySpan<char> source = "42".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        NumberExpressionNode func = Assert.IsType<NumberExpressionNode>(node);
        Assert.Equal("42", source[(Range)func.Constant]);
    }
    [Fact]
    public void IdentifierExpressionParenthesis()
    {
        ReadOnlySpan<char> source = "(test)".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        IdentifierExpressionNode func = Assert.IsType<IdentifierExpressionNode>(node);
        IVariableSegment first = Assert.Single(func.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("test", source[(Range)member.MemberName]);
    }
    [Theory]
    [InlineData("func", "test")]
    //[InlineData("func", "test[0]")]
    public void FunctionExpression(string funcName, string ident)
    {
        ReadOnlySpan<char> source = $"{funcName}({ident})".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal(funcName, source[(Range)func.FunctionName]);
        IExpressionNode arg = Assert.Single(func.Arguments);
        IdentifierExpressionNode identifier = Assert.IsType<IdentifierExpressionNode>(arg);
        IVariableSegment first = Assert.Single(identifier.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal(ident, source[(Range)member.MemberName]);
    }



    [Fact]
    public void FunctionNestedExpression()
    {
        ReadOnlySpan<char> source = $"func(one nested(two))".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", source[(Range)func.FunctionName]);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierExpressionNode ident = Assert.IsType<IdentifierExpressionNode>(func.Arguments[0]);
        IVariableSegment first = Assert.Single(ident.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("one", source[(Range)member.MemberName]);
        FunctionCallNode nested = Assert.IsType<FunctionCallNode>(func.Arguments[1]);
        Assert.Equal("nested", source[(Range)nested.FunctionName]);
        IExpressionNode nestedArg = Assert.Single(nested.Arguments);
        IdentifierExpressionNode nestedIdent = Assert.IsType<IdentifierExpressionNode>(nestedArg);
        IVariableSegment one= Assert.Single(nestedIdent.Path.Segments);
        MemberSegment memberone = Assert.IsType<MemberSegment>(one);
        Assert.Equal("two", source[(Range)memberone.MemberName]);
    }

    [Fact]
    public void FunctionManyArgs()
    {
        ReadOnlySpan<char> source = "func(one two)".AsSpan();
        NodeLexer lexer = new(source);
        ExpressionLexer eLexer = new(ref lexer, Extract.Full(source.Length));
        IExpressionNode? node = eLexer.Parse();
        FunctionCallNode func = Assert.IsType<FunctionCallNode>(node);
        Assert.Equal("func", source[(Range)func.FunctionName]);
        Assert.Equal(2, func.Arguments.Length);
        IdentifierExpressionNode ident1 = Assert.IsType<IdentifierExpressionNode>(func.Arguments[0]);
        IVariableSegment first = Assert.Single(ident1.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("one", source[(Range)member.MemberName]);
        IdentifierExpressionNode ident2 = Assert.IsType<IdentifierExpressionNode>(func.Arguments[1]);
        IVariableSegment two = Assert.Single(ident2.Path.Segments);
        MemberSegment member2 = Assert.IsType<MemberSegment>(two);
        Assert.Equal("two", source[(Range)member2.MemberName]);
    }

    //[Fact]
    //public void FunctionManyArgs__Malformed()
    //{
    //    try
    //    {
    //        _ = "func(one two".AsSpan().ParseExpression();
    //    }
    //    catch (Exception)
    //    {
    //        Assert.True(true, "Expected exception thrown");
    //    }
    //}
}
