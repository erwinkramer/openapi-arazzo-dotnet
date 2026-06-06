using System.Text.Json.Nodes;

using Microsoft.OpenApi;

using OpenApiJsonNodeExtension = Microsoft.OpenApi.JsonNodeExtension;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents a JSON Schema-based input definition in an Arazzo document.
/// </summary>
public class ArazzoInput : IArazzoInput
{
    /// <inheritdoc />
    public string? Title { get; set; }

    /// <inheritdoc />
    public Uri? Schema { get; set; }

    /// <inheritdoc />
    public string? Id { get; set; }

    /// <inheritdoc />
    public string? Comment { get; set; }

    /// <inheritdoc />
    public IDictionary<string, bool>? Vocabulary { get; set; }

    /// <inheritdoc />
    public string? DynamicRef { get; set; }

    /// <inheritdoc />
    public string? DynamicAnchor { get; set; }

    /// <inheritdoc />
    public IDictionary<string, IArazzoInput>? Definitions { get; set; }

    /// <inheritdoc />
    public string? ExclusiveMaximum { get; set; }

    /// <inheritdoc />
    public string? ExclusiveMinimum { get; set; }

    /// <inheritdoc />
    public JsonSchemaType? Type { get; set; }

    /// <inheritdoc />
    public string? Const { get; set; }

    /// <inheritdoc />
    public string? Format { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public string? Maximum { get; set; }

    /// <inheritdoc />
    public string? Minimum { get; set; }

    /// <inheritdoc />
    public int? MaxLength { get; set; }

    /// <inheritdoc />
    public int? MinLength { get; set; }

    /// <inheritdoc />
    public string? Pattern { get; set; }

    /// <inheritdoc />
    public decimal? MultipleOf { get; set; }

    /// <inheritdoc />
    public JsonNode? Default { get; set; }

    /// <inheritdoc />
    public bool ReadOnly { get; set; }

    /// <inheritdoc />
    public bool WriteOnly { get; set; }

    /// <inheritdoc />
    public IList<IArazzoInput>? AllOf { get; set; }

    /// <inheritdoc />
    public IList<IArazzoInput>? OneOf { get; set; }

    /// <inheritdoc />
    public IList<IArazzoInput>? AnyOf { get; set; }

    /// <inheritdoc />
    public IArazzoInput? Not { get; set; }

    /// <inheritdoc />
    public ISet<string>? Required { get; set; }

    /// <inheritdoc />
    public IArazzoInput? Items { get; set; }

    /// <inheritdoc />
    public int? MaxItems { get; set; }

    /// <inheritdoc />
    public int? MinItems { get; set; }

    /// <inheritdoc />
    public bool? UniqueItems { get; set; }

    /// <inheritdoc />
    public IDictionary<string, IArazzoInput>? Properties { get; set; }

    /// <inheritdoc />
    public IDictionary<string, IArazzoInput>? PatternProperties { get; set; }

    /// <inheritdoc />
    public int? MaxProperties { get; set; }

    /// <inheritdoc />
    public int? MinProperties { get; set; }

    /// <inheritdoc />
    public bool AdditionalPropertiesAllowed { get; set; } = true;

    /// <inheritdoc />
    public IArazzoInput? AdditionalProperties { get; set; }

    /// <inheritdoc />
    public IList<JsonNode>? Examples { get; set; }

    /// <inheritdoc />
    public IList<JsonNode>? Enum { get; set; }

    /// <inheritdoc />
    public bool UnevaluatedProperties { get; set; } = true;

    /// <inheritdoc />
    public IArazzoInput? UnevaluatedPropertiesSchema { get; set; }

    /// <inheritdoc />
    public bool Deprecated { get; set; }

    /// <inheritdoc />
    public IDictionary<string, HashSet<string>>? DependentRequired { get; set; }

    /// <inheritdoc />
    public IDictionary<string, IArazzoExtension>? Extensions { get; set; }

    /// <inheritdoc />
    public void SerializeAsV1(IOpenApiWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        ToOpenApiSchema(this).SerializeAsV32(writer);
    }

    /// <summary>
    /// Converts an OpenAPI schema to an Arazzo input.
    /// </summary>
    /// <param name="value">The OpenAPI schema to convert.</param>
    public static implicit operator ArazzoInput?(OpenApiSchema? value)
    {
        if (value is null)
        {
            return null;
        }

        ValidateUnsupportedOpenApiKeywords(value);

        return FromOpenApiSchema(value);
    }

    /// <summary>
    /// Converts an Arazzo input to an OpenAPI schema.
    /// </summary>
    /// <param name="value">The Arazzo input to convert.</param>
    public static implicit operator OpenApiSchema?(ArazzoInput? value)
    {
        if (value is null)
        {
            return null;
        }

        return ToOpenApiSchema(value);
    }

    private static ArazzoInput FromOpenApiSchema(IOpenApiSchema schema)
    {
        if (schema is OpenApiSchema openApiSchema)
        {
            ValidateUnsupportedOpenApiKeywords(openApiSchema);
        }
        else
        {
            throw new NotSupportedException($"Conversion from {schema.GetType().Name} is not supported.");
        }

        return new ArazzoInput
        {
            Title = schema.Title,
            Schema = schema.Schema,
            Id = schema.Id,
            Comment = schema.Comment,
            Vocabulary = schema.Vocabulary is null ? null : new Dictionary<string, bool>(schema.Vocabulary),
            DynamicRef = schema.DynamicRef,
            DynamicAnchor = schema.DynamicAnchor,
            Definitions = ConvertSchemaMap(schema.Definitions),
            ExclusiveMaximum = schema.ExclusiveMaximum,
            ExclusiveMinimum = schema.ExclusiveMinimum,
            Type = schema.Type,
            Const = schema.Const,
            Format = schema.Format,
            Description = schema.Description,
            Maximum = schema.Maximum,
            Minimum = schema.Minimum,
            MaxLength = schema.MaxLength,
            MinLength = schema.MinLength,
            Pattern = schema.Pattern,
            MultipleOf = schema.MultipleOf,
            Default = CloneNode(schema.Default),
            ReadOnly = schema.ReadOnly,
            WriteOnly = schema.WriteOnly,
            AllOf = ConvertSchemaList(schema.AllOf),
            OneOf = ConvertSchemaList(schema.OneOf),
            AnyOf = ConvertSchemaList(schema.AnyOf),
            Not = schema.Not is null ? null : FromOpenApiSchema(schema.Not),
            Required = schema.Required is null ? null : new HashSet<string>(schema.Required),
            Items = schema.Items is null ? null : FromOpenApiSchema(schema.Items),
            MaxItems = schema.MaxItems,
            MinItems = schema.MinItems,
            UniqueItems = schema.UniqueItems,
            Properties = ConvertSchemaMap(schema.Properties),
            PatternProperties = ConvertSchemaMap(schema.PatternProperties),
            MaxProperties = schema.MaxProperties,
            MinProperties = schema.MinProperties,
            AdditionalPropertiesAllowed = schema.AdditionalPropertiesAllowed,
            AdditionalProperties = schema.AdditionalProperties is null ? null : FromOpenApiSchema(schema.AdditionalProperties),
            Examples = CloneNodeList(schema.Examples),
            Enum = CloneNodeList(schema.Enum),
            UnevaluatedProperties = openApiSchema.UnevaluatedProperties,
            UnevaluatedPropertiesSchema = openApiSchema.UnevaluatedPropertiesSchema is null ? null : FromOpenApiSchema(openApiSchema.UnevaluatedPropertiesSchema),
            Deprecated = schema.Deprecated,
            Extensions = ConvertExtensions(openApiSchema.Extensions),
            DependentRequired = CloneDependentRequired(schema.DependentRequired)
        };
    }

    private static OpenApiSchema ToOpenApiSchema(IArazzoInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new OpenApiSchema
        {
            Title = input.Title,
            Schema = input.Schema,
            Id = input.Id,
            Comment = input.Comment,
            Vocabulary = input.Vocabulary is null ? null : new Dictionary<string, bool>(input.Vocabulary),
            DynamicRef = input.DynamicRef,
            DynamicAnchor = input.DynamicAnchor,
            Definitions = ConvertSchemaMap(input.Definitions),
            ExclusiveMaximum = input.ExclusiveMaximum,
            ExclusiveMinimum = input.ExclusiveMinimum,
            Type = input.Type,
            Const = input.Const,
            Format = input.Format,
            Description = input.Description,
            Maximum = input.Maximum,
            Minimum = input.Minimum,
            MaxLength = input.MaxLength,
            MinLength = input.MinLength,
            Pattern = input.Pattern,
            MultipleOf = input.MultipleOf,
            Default = CloneNode(input.Default),
            ReadOnly = input.ReadOnly,
            WriteOnly = input.WriteOnly,
            AllOf = ConvertSchemaList(input.AllOf),
            OneOf = ConvertSchemaList(input.OneOf),
            AnyOf = ConvertSchemaList(input.AnyOf),
            Not = input.Not is null ? null : ToOpenApiSchema(input.Not),
            Required = input.Required is null ? null : new HashSet<string>(input.Required),
            Items = input.Items is null ? null : ToOpenApiSchema(input.Items),
            MaxItems = input.MaxItems,
            MinItems = input.MinItems,
            UniqueItems = input.UniqueItems,
            Properties = ConvertSchemaMap(input.Properties),
            PatternProperties = ConvertSchemaMap(input.PatternProperties),
            MaxProperties = input.MaxProperties,
            MinProperties = input.MinProperties,
            AdditionalPropertiesAllowed = input.AdditionalPropertiesAllowed,
            AdditionalProperties = input.AdditionalProperties is null ? null : ToOpenApiSchema(input.AdditionalProperties),
            Examples = CloneNodeList(input.Examples),
            Enum = CloneNodeList(input.Enum),
            UnevaluatedProperties = input.UnevaluatedProperties,
            UnevaluatedPropertiesSchema = input.UnevaluatedPropertiesSchema is null ? null : ToOpenApiSchema(input.UnevaluatedPropertiesSchema),
            Deprecated = input.Deprecated,
            Extensions = ConvertExtensions(input.Extensions),
            DependentRequired = CloneDependentRequired(input.DependentRequired)
        };
    }

    private static void ValidateUnsupportedOpenApiKeywords(OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        var unsupportedKeywords = new List<string>();

        if (schema.Discriminator is not null)
        {
            unsupportedKeywords.Add(nameof(schema.Discriminator));
        }

        if (schema.Example is not null)
        {
            unsupportedKeywords.Add(nameof(schema.Example));
        }

        if (schema.ExternalDocs is not null)
        {
            unsupportedKeywords.Add(nameof(schema.ExternalDocs));
        }

        if (schema.Xml is not null)
        {
            unsupportedKeywords.Add(nameof(schema.Xml));
        }

        if (unsupportedKeywords.Count > 0)
        {
            throw new InvalidOperationException(
                $"OpenAPI-specific schema keywords are not supported by {nameof(ArazzoInput)}: {string.Join(", ", unsupportedKeywords)}.");
        }
    }

    private static IDictionary<string, IArazzoInput>? ConvertSchemaMap(
        IDictionary<string, IOpenApiSchema>? source)
    {
        if (source is null)
        {
            return null;
        }

        return source.ToDictionary(static pair => pair.Key, static pair => (IArazzoInput)FromOpenApiSchema(pair.Value));
    }

    private static IDictionary<string, IOpenApiSchema>? ConvertSchemaMap(
        IDictionary<string, IArazzoInput>? source)
    {
        if (source is null)
        {
            return null;
        }

        return source.ToDictionary(static pair => pair.Key, static pair => (IOpenApiSchema)ToOpenApiSchema(pair.Value));
    }

    private static IList<IArazzoInput>? ConvertSchemaList(
        IList<IOpenApiSchema>? source)
    {
        if (source is null)
        {
            return null;
        }

        return source.Select(static schema => (IArazzoInput)FromOpenApiSchema(schema)).ToList();
    }

    private static IList<IOpenApiSchema>? ConvertSchemaList(
        IList<IArazzoInput>? source)
    {
        if (source is null)
        {
            return null;
        }

        return source.Select(static schema => (IOpenApiSchema)ToOpenApiSchema(schema)).ToList();
    }

    private static JsonNode? CloneNode(JsonNode? node)
    {
        if (node is null)
        {
            return null;
        }
        return JsonNode.Parse(node.ToJsonString());
    }

    private static IList<JsonNode>? CloneNodeList(IList<JsonNode>? nodes)
    {
        if (nodes is null)
        {
            return null;
        }

        return nodes.Select(static node => CloneNode(node)!).ToList();
    }

    private static IDictionary<string, HashSet<string>>? CloneDependentRequired(IDictionary<string, HashSet<string>>? dependentRequired)
    {
        if (dependentRequired is null)
        {
            return null;
        }

        return dependentRequired.ToDictionary(static pair => pair.Key, static pair => new HashSet<string>(pair.Value));
    }

    private static IDictionary<string, IArazzoExtension>? ConvertExtensions(IDictionary<string, IOpenApiExtension>? extensions)
    {
        if (extensions is null)
        {
            return null;
        }

        return extensions.ToDictionary(
            static pair => pair.Key,
            static pair => pair.Value switch
            {
                OpenApiJsonNodeExtension jsonNodeExtension => (IArazzoExtension)new JsonNodeExtension(CloneNode(jsonNodeExtension.Node)!),
                _ => new ArazzoExtensionAdapter(pair.Value)
            });
    }

    private static IDictionary<string, IOpenApiExtension>? ConvertExtensions(IDictionary<string, IArazzoExtension>? extensions)
    {
        if (extensions is null)
        {
            return null;
        }

        return extensions.ToDictionary(
            static pair => pair.Key,
            static pair => pair.Value switch
            {
                JsonNodeExtension jsonNodeExtension => (IOpenApiExtension)new OpenApiJsonNodeExtension(CloneNode(jsonNodeExtension.Node)!),
                _ => new OpenApiExtensionAdapter(pair.Value)
            });
    }

    private sealed class ArazzoExtensionAdapter(IOpenApiExtension extension) : IArazzoExtension
    {
        public void Write(IOpenApiWriter writer, ArazzoSpecVersion specVersion)
        {
            extension.Write(writer, OpenApiSpecVersion.OpenApi3_2);
        }
    }

    private sealed class OpenApiExtensionAdapter(IArazzoExtension extension) : IOpenApiExtension
    {
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            extension.Write(writer, ArazzoSpecVersion.Arazzo1_0);
        }
    }
}