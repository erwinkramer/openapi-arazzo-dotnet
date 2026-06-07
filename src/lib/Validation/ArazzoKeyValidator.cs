using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using BinkyLabs.OpenApi.Arazzo.Reader;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo.Validation;

internal static partial class ArazzoKeyValidator
{
    private const string ValidKeyPattern = "^[a-zA-Z0-9\\.\\-_]+$";

    [GeneratedRegex(ValidKeyPattern, RegexOptions.CultureInvariant)]
    private static partial Regex ValidKeyRegex();

    internal static void ValidateSerializationKeys(IEnumerable<string>? keys, string collectionName)
    {
        if (keys is null)
        {
            return;
        }
        foreach (var key in keys)
        {
            if (!ValidKeyRegex().IsMatch(key))
            {
                throw new ArazzoSerializationException($"Keys in {collectionName} must match regex {ValidKeyPattern}. Invalid key: '{key}'.");
            }
        }
    }

    internal static void ValidateDeserializationKeys(JsonNode? node, ParsingContext context, string collectionName)
    {
        if (node is not JsonObject jsonObject)
        {
            return;
        }

        foreach (var key in jsonObject.Select(static x => x.Key))
        {
            if (!ValidKeyRegex().IsMatch(key))
            {
                context.Diagnostic.Errors.Add(new OpenApiError($"{context.GetLocation()}/{EscapePointerSegment(key)}", $"Keys in {collectionName} must match regex {ValidKeyPattern}. Invalid key: '{key}'."));
            }
        }
    }

    private static string EscapePointerSegment(string segment)
    {
        return segment.Replace("~", "~0", StringComparison.Ordinal).Replace("/", "~1", StringComparison.Ordinal);
    }
}