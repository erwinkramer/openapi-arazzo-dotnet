namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    public static readonly FixedFieldMap<ArazzoRequestBody> RequestBodyFixedFields = new()
    {
        { ArazzoConstants.ArazzoRequestBodyContentType, (o, v) => o.ContentType = v.GetScalarValue() },
        { ArazzoConstants.ArazzoRequestBodyPayload, (o, v) => o.Payload = v.CreateAny() },
        { ArazzoConstants.ArazzoRequestBodyReplacements, (o, v) => o.Replacements = v.CreateList(LoadPayloadReplacement) }
    };

    public static readonly PatternFieldMap<ArazzoRequestBody> RequestBodyPatternFields = new()
    {
        { s => s.StartsWith(ArazzoConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n) => o.AddExtension(k, LoadExtension(k, n)) }
    };

    public static ArazzoRequestBody LoadRequestBody(ParseNode node)
    {
        var mapNode = node.CheckMapNode("RequestBody");
        var requestBody = new ArazzoRequestBody();
        ParseMap(mapNode, requestBody, RequestBodyFixedFields, RequestBodyPatternFields);

        return requestBody;
    }
}