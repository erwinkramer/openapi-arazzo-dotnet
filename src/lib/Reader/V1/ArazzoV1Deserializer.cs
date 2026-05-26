using System.Text.Json.Nodes;

namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    private static void ParseMap<T>(
        JsonObject? mapNode,
        T domainObject,
        FixedFieldMap<T> fixedFieldMap,
        PatternFieldMap<T> patternFieldMap,
        ParsingContext context)
    {
        mapNode.ParseMap(domainObject, fixedFieldMap, patternFieldMap, context);
    }

    public static JsonNode LoadAny(JsonNode node, ParsingContext context)
    {
        return node.CreateAny();
    }

    private static IArazzoExtension LoadExtension(string name, JsonNode node, ParsingContext context)
    {
        if (context.ExtensionParsers is not null && context.ExtensionParsers.TryGetValue(name, out var parser) && parser(
            node.CreateAny(), ArazzoSpecVersion.Arazzo1_0) is { } result)
        {
            return result;
        }
        else
        {
            return new JsonNodeExtension(node.CreateAny());
        }
    }
}