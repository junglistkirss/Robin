using System.Text;

namespace RobinMustache.Generators.Accessor
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIndented(
            this StringBuilder sb,
            int indentLevel,
            string text)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            if (indentLevel < 0)
                indentLevel = 0;

            // Apply indentation before appending text
            for (int i = 0; i < indentLevel; i++)
                sb.Append("    ");

            sb.AppendLine(text);
            return sb;
        }

        public static StringBuilder AppendLinesIndented(
             this StringBuilder sb,
             int indentLevel,
             params string[] lines)
        {
            foreach (var line in lines)
                sb.AppendLineIndented(indentLevel, line);

            return sb;
        }
    }
}