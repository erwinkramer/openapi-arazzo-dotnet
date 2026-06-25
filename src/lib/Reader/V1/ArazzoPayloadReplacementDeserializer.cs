using System.Text.Json.Nodes;

using BinkyLabs.OpenApi.Arazzo.Validation;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    public static readonly FixedFieldMap<ArazzoPayloadReplacement> PayloadReplacementFixedFields = new()
    {
        { ArazzoConstants.ArazzoPayloadReplacementTarget, static (o, v, c) => o.Target = v.GetScalarValue() },
        { ArazzoConstants.ArazzoPayloadReplacementValue, static (o, v, c) => o.Value = v }
    };

    public static readonly PatternFieldMap<ArazzoPayloadReplacement> PayloadReplacementPatternFields = new()
    {
        { s => s.StartsWith(ArazzoConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n, c) => o.AddExtension(k, LoadExtension(k, n, c)) }
    };

    public static ArazzoPayloadReplacement LoadPayloadReplacement(JsonNode node, ParsingContext context)
    {
        var mapNode = node.CheckMapNode("PayloadReplacement", context);
        var replacement = new ArazzoPayloadReplacement();
        mapNode.ParseMap(replacement, PayloadReplacementFixedFields, PayloadReplacementPatternFields, context);
        ValidatePayloadReplacementRequiredFields(replacement, context);
        ArazzoRuntimeExpressionValidator.ValidateDeserializationExpressionStrings(replacement.Value, context, $"{nameof(ArazzoPayloadReplacement)}.{nameof(ArazzoPayloadReplacement.Value)}");

        return replacement;
    }

    private static void ValidatePayloadReplacementRequiredFields(ArazzoPayloadReplacement replacement, ParsingContext context)
    {
        if (string.IsNullOrEmpty(replacement.Target))
        {
            context.Diagnostic.Errors.Add(new OpenApiError(context.GetLocation(), $"{nameof(ArazzoPayloadReplacement)}.{nameof(ArazzoPayloadReplacement.Target)} is a REQUIRED field."));
        }

        if (replacement.Value is null)
        {
            context.Diagnostic.Errors.Add(new OpenApiError(context.GetLocation(), $"{nameof(ArazzoPayloadReplacement)}.{nameof(ArazzoPayloadReplacement.Value)} is a REQUIRED field."));
        }
    }
}