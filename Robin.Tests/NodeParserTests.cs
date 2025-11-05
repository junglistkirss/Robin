using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
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
        Assert.Equal("text", source[(Range)textNode.Text]);
    }


    [Fact]
    public void SimpleComment()
    {
        ReadOnlySpan<char> source = "{{!comment}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        CommentNode com = Assert.IsType<CommentNode>(node);
        Assert.Equal("comment", source[(Range)com.Range]);
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
        IVariableSegment first = Assert.Single(id.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("var", source[(Range)member.MemberName]);
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
        IVariableSegment first = Assert.Single(id.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("esc", source[(Range)member.MemberName]);
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
        IVariableSegment first = Assert.Single(id.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("sec", source[(Range)member.MemberName]);
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
        IVariableSegment first = Assert.Single(id.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("inv", source[(Range)member.MemberName]);
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
        IVariableSegment first = Assert.Single(block.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("block", source[(Range)member.MemberName]);
        INode content = Assert.Single(section.Children);
        TextNode contentText = Assert.IsType<TextNode>(content);
        Assert.Equal("content", source[(Range)contentText.Text]);
    }

    [Fact]
    public void EmptyPartial()
    {
        ReadOnlySpan<char> source = "{{<partial}}{{/partial}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialDefineNode partial = Assert.IsType<PartialDefineNode>(node);
        Assert.Equal("partial", source[(Range)partial.PartialName]);
        Assert.Empty(partial.Children);
    }

    [Fact]
    public void NotEmptyPartial()
    {
        ReadOnlySpan<char> source = "{{<block}}content{{/block}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialDefineNode partial = Assert.IsType<PartialDefineNode>(node);
        Assert.Equal("block", source[(Range)partial.PartialName]);
        INode content = Assert.Single(partial.Children);
        TextNode contentText = Assert.IsType<TextNode>(content);
        Assert.Equal("content", source[(Range)contentText.Text]);
    }

    [Fact]
    public void CallPartialThis()
    {
        ReadOnlySpan<char> source = "{{>partial}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialCallNode partial = Assert.IsType<PartialCallNode>(node);
        Assert.Equal("partial", source[(Range)partial.PartialName]);
        Assert.NotNull(partial.Expression);
        IdentifierExpressionNode block = Assert.IsType<IdentifierExpressionNode>(partial.Expression);
        IVariableSegment first = Assert.Single(block.Path.Segments);
        ThisSegment member = Assert.IsType<ThisSegment>(first);
    }

    [Fact]
    public void CallPartialVariable()
    {
        ReadOnlySpan<char> source = "{{> partial test }}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        PartialCallNode partial = Assert.IsType<PartialCallNode>(node);
        Assert.Equal("partial", source[(Range)partial.PartialName]);
        Assert.NotNull(partial.Expression);
        IdentifierExpressionNode block = Assert.IsType<IdentifierExpressionNode>(partial.Expression);
        IVariableSegment first = Assert.Single(block.Path.Segments);
        MemberSegment member = Assert.IsType<MemberSegment>(first);
        Assert.Equal("test", source[(Range)member.MemberName]);
    }
}
