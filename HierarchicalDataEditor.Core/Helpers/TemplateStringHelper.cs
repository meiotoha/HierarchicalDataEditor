using System.Text;
using System.Text.RegularExpressions;

namespace HierarchicalDataEditor.Core.Helpers
{
    public static class TemplateStringHelper
    {
        public static string ParseText(this string template, params object[] context)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }
            var sb = new StringBuilder(template);
            var keys = Regex.Matches(template, @"\{\w+\}|{\.}");
            foreach (Match match in keys)
            {
                sb.Replace(match.Value, context.GetValue(match.Value));
            }
            return sb.ToString();
        }
        private static string GetValue(this object[] contexts, string name, string fallback = default)
        {
            return contexts.TryGetFirstOrDefaultValue(name)?.ToString() ?? fallback;
        }
    }
}
