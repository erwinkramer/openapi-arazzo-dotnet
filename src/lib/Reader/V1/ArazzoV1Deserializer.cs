using System.Text.Json.Nodes;

namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    private static void ParseMap<T>(
        MapNode mapNode,
        T domainObject,
        FixedFieldMap<T> fixedFieldMap,
        PatternFieldMap<T> patternFieldMap)
    {
        foreach (var propertyNode in mapNode)
        {
            propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap);
        }

    }
    public static JsonNode LoadAny(ParseNode node)
    {
        return node.CreateAny();
    }
    private static IArazzoExtension LoadExtension(string name, ParseNode node)
    {
        if (node.Context.ExtensionParsers is not null && node.Context.ExtensionParsers.TryGetValue(name, out var parser) && parser(
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