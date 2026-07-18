using System.Collections;
using System.Text.RegularExpressions;

namespace LMS.Infrastructure.Email;

/// <summary>
/// Minimal <c>{{token}}</c> template renderer replacing the original EJS templates.
/// Supports dotted paths (e.g. <c>{{user.name}}</c>, <c>{{order.price}}</c>) resolved
/// against nested dictionaries and anonymous objects.
/// </summary>
public static class EmailTemplateRenderer
{
    private static readonly Regex TokenPattern = new(@"\{\{\s*([\w\.]+)\s*\}\}", RegexOptions.Compiled);

    public static string Render(string template, IDictionary<string, object?> data)
        => TokenPattern.Replace(template, match =>
        {
            var path = match.Groups[1].Value;
            var value = Resolve(data, path);
            return value?.ToString() ?? string.Empty;
        });

    private static object? Resolve(object? current, string path)
    {
        foreach (var segment in path.Split('.'))
        {
            current = current switch
            {
                null => null,
                IDictionary<string, object?> dict => dict.TryGetValue(segment, out var v) ? v : null,
                IDictionary legacy => legacy.Contains(segment) ? legacy[segment] : null,
                _ => current.GetType().GetProperty(segment)?.GetValue(current),
            };

            if (current is null) return null;
        }

        return current;
    }
}
