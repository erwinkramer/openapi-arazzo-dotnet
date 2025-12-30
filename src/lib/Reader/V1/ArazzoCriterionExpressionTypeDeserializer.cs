using System;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    public static readonly FixedFieldMap<ArazzoCriterionExpressionType> CriterionExpressionTypeFixedFields = new()
    {
        { ArazzoConstants.ArazzoCriterionExpressionTypeType, (o, v) =>
        {
            if (!v.GetScalarValue().TryGetEnumFromDisplayName<ArazzoCriterionExpressionTypeType>(v.Context, out var type))
            {
                return;
            }
            o.Type = type;
        } },
        { ArazzoConstants.ArazzoCriterionExpressionTypeVersion, (o, v) =>
        {
            if (!v.GetScalarValue().TryGetEnumFromDisplayName<ArazzoCriterionExpressionVersion>(v.Context, out var version))
            {
                return;
            }
            o.Version = version;
        } }
    };

    public static readonly PatternFieldMap<ArazzoCriterionExpressionType> CriterionExpressionTypePatternFields = new()
    {
        { s => s.StartsWith(ArazzoConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n) => o.AddExtension(k, LoadExtension(k, n)) }
    };

    public static ArazzoCriterionExpressionType LoadCriterionExpressionType(ParseNode node)
    {
        var mapNode = node.CheckMapNode("CriterionExpressionType");
        var expressionType = new ArazzoCriterionExpressionType();
        ParseMap(mapNode, expressionType, CriterionExpressionTypeFixedFields, CriterionExpressionTypePatternFields);

        return expressionType;
    }
}
