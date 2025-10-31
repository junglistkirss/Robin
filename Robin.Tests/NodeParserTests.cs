using Robin.Expressions;
using Robin.Nodes;
using System.Collections.Immutable;

namespace Robin.tests;

public class NodeParserTests
{

    [Fact]
    public void EmptyTemplate()
    {
        ReadOnlySpan<char> source = "".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        Assert.Empty(nodes);
    }

    [Fact]
    public void SimpleText()
    {
        ReadOnlySpan<char> source = "text".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        TextNode textNode = Assert.IsType<TextNode>(node);
        Assert.Equal("text", textNode.Text);
    }


    [Fact]
    public void SimpleComment()
    {
        ReadOnlySpan<char> source = "{{!comment}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        CommentNode com = Assert.IsType<CommentNode>(node);
        Assert.Equal("comment", com.Message);
    }

    [Fact]
    public void SimpleVariable()
    {
        ReadOnlySpan<char> source = "{{var}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        VariableNode var = Assert.IsType<VariableNode>(node);
        Assert.False(var.IsUnescaped);
        Assert.NotNull(var.Expression);
        IdentifierExpressionNode id = Assert.IsType<IdentifierExpressionNode>(var.Expression);
        Assert.Equal("var", id.Path);
    }

    [Fact]
    public void SimpleUnescapedVariable()
    {
        ReadOnlySpan<char> source = "{{{esc}}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        VariableNode esc = Assert.IsType<VariableNode>(node);
        Assert.True(esc.IsUnescaped);
        Assert.NotNull(esc.Expression);
        IdentifierExpressionNode id = Assert.IsType<IdentifierExpressionNode>(esc.Expression);
        Assert.Equal("esc", id.Path);
    }

    [Fact]
    public void EmptySection()
    {
        ReadOnlySpan<char> source = "{{#sec}}{{/sec}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        SectionNode section = Assert.IsType<SectionNode>(node);
        Assert.False(section.Inverted);
        Assert.Empty(section.Children);
        Assert.NotNull(section.Expression);
        IdentifierExpressionNode id = Assert.IsType<IdentifierExpressionNode>(section.Expression);
        Assert.Equal("sec", id.Path);
    }

    [Fact]
    public void EmptyInvertedSection()
    {
        ReadOnlySpan<char> source = "{{^inv}}{{/inv}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        SectionNode section = Assert.IsType<SectionNode>(node);
        Assert.True(section.Inverted);
        Assert.Empty(section.Children);
        Assert.NotNull(section.Expression);
        IdentifierExpressionNode id = Assert.IsType<IdentifierExpressionNode>(section.Expression);
        Assert.Equal("inv", id.Path);
    }

    [Fact]
    public void NotEmptySection()
    {
        ReadOnlySpan<char> source = "{{#block}}content{{/block}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        SectionNode section = Assert.IsType<SectionNode>(node);
        Assert.False(section.Inverted);
        Assert.NotNull(section.Expression);
        IdentifierExpressionNode block = Assert.IsType<IdentifierExpressionNode>(section.Expression);
        Assert.Equal("block", block.Path);
        INode content = Assert.Single(section.Children);
        TextNode contentText = Assert.IsType<TextNode>(content);
        Assert.Equal("content", contentText.Text);
    }

    [Fact]
    public void EmptyPartial()
    {
        ReadOnlySpan<char> source = "{{>partial}}{{/partial}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialNode partial = Assert.IsType<PartialNode>(node);
        Assert.Equal("partial", partial.Name);
        Assert.Empty(partial.Children);
    }

    [Fact]
    public void NotEmptyPartial()
    {
        ReadOnlySpan<char> source = "{{>block}}content{{/block}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialNode partial = Assert.IsType<PartialNode>(node);
        Assert.Equal("block", partial.Name);
        INode content = Assert.Single(partial.Children);
        TextNode contentText = Assert.IsType<TextNode>(content);
        Assert.Equal("content", contentText.Text);
    }

}
