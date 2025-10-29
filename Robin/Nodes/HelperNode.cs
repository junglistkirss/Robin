using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct HelperNode(ReadOnlyMemory<char> name, INode argument) : INode
{
    public ReadOnlyMemory<char> Name { get; } = name;
    public INode Argument { get; } = argument;

    public void Render(Context context, StringBuilder output)
    {
        if (context.Helpers.TryGetValue(Name.ToString(), out Action<Context, INode, StringBuilder>? helper))
        {
            helper(context, Argument, output);
        }
    }
}

