using System.Text.Json.Nodes;

using BinkyLabs.OpenApi.Arazzo.Reader;
using BinkyLabs.OpenApi.Arazzo.Reader.V1;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo.Tests;

public class ArazzoCriterionExpressionTypeTests
{
    [Fact]
    public void SerializeAsV1_WithJsonPathType_ShouldWriteCorrectJson()
    {
        var expressionType = new ArazzoCriterionExpressionType
        {
            Type = ArazzoCriterionExpressionTypeType.JsonPath,
            Version = ArazzoCriterionExpressionVersion.DraftGoessnerDispatchJsonPath00,
            Extensions = new Dictionary<string, IArazzoExtension>
            {
                ["x-extra"] = new JsonNodeExtension(JsonNode.Parse("{\"note\":\"yes\"}")!)
            }
        };
        using var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        var expectedJson =
        """
        {
            "type": "jsonpath",
            "version": "draft-goessner-dispatch-jsonpath-00",
            "x-extra": {
                "note": "yes"
            }
        }
        """;

        expressionType.SerializeAsV1(writer);
        var jsonResultObject = JsonNode.Parse(textWriter.ToString());
        var expectedJsonObject = JsonNode.Parse(expectedJson);

        Assert.True(JsonNode.DeepEquals(jsonResultObject, expectedJsonObject), "Serialized JSON does not match expected output.");
    }

    [Fact]
    public void SerializeAsV1_WithXPathType_ShouldWriteCorrectJson()
    {
        var expressionType = new ArazzoCriterionExpressionType
        {
            Type = ArazzoCriterionExpressionTypeType.XPath,
            Version = ArazzoCriterionExpressionVersion.XPath30
        };
        using var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        var expectedJson =
        """
        {
            "type": "xpath",
            "version": "xpath-30"
        }
        """;

        expressionType.SerializeAsV1(writer);
        var jsonResultObject = JsonNode.Parse(textWriter.ToString());
        var expectedJsonObject = JsonNode.Parse(expectedJson);

        Assert.True(JsonNode.DeepEquals(jsonResultObject, expectedJsonObject), "Serialized JSON does not match expected output.");
    }

    [Fact]
    public void Deserialize_ShouldSetPropertiesAndExtensions()
    {
        var json = """
        {
            "type": "xpath",
            "version": "xpath-20",
            "x-flag": true
        }
        """;
        var jsonNode = JsonNode.Parse(json)!;
        var parsingContext = new ParsingContext(new());
        var parseNode = new MapNode(parsingContext, jsonNode);

        var expressionType = ArazzoV1Deserializer.LoadCriterionExpressionType(parseNode);

        Assert.Equal(ArazzoCriterionExpressionTypeType.XPath, expressionType.Type);
        Assert.Equal(ArazzoCriterionExpressionVersion.XPath20, expressionType.Version);
        Assert.NotNull(expressionType.Extensions);
        var extension = Assert.IsType<JsonNodeExtension>(expressionType.Extensions!["x-flag"]);
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse("true"), extension.Node));
    }

    [Fact]
    public void Deserialize_WithJsonPath_ShouldSetPropertiesCorrectly()
    {
        var json = """
        {
            "type": "jsonpath",
            "version": "draft-goessner-dispatch-jsonpath-00"
        }
        """;
        var jsonNode = JsonNode.Parse(json)!;
        var parsingContext = new ParsingContext(new());
        var parseNode = new MapNode(parsingContext, jsonNode);

        var expressionType = ArazzoV1Deserializer.LoadCriterionExpressionType(parseNode);

        Assert.Equal(ArazzoCriterionExpressionTypeType.JsonPath, expressionType.Type);
        Assert.Equal(ArazzoCriterionExpressionVersion.DraftGoessnerDispatchJsonPath00, expressionType.Version);
    }

    [Fact]
    public void Deserialize_WithXPath10_ShouldSetPropertiesCorrectly()
    {
        var json = """
        {
            "type": "xpath",
            "version": "xpath-10"
        }
        """;
        var jsonNode = JsonNode.Parse(json)!;
        var parsingContext = new ParsingContext(new());
        var parseNode = new MapNode(parsingContext, jsonNode);

        var expressionType = ArazzoV1Deserializer.LoadCriterionExpressionType(parseNode);

        Assert.Equal(ArazzoCriterionExpressionTypeType.XPath, expressionType.Type);
        Assert.Equal(ArazzoCriterionExpressionVersion.XPath10, expressionType.Version);
    }

    [Fact]
    public void SerializeAsV1_WithoutTypeThrowsException()
    {
        var expressionType = new ArazzoCriterionExpressionType
        {
            Version = ArazzoCriterionExpressionVersion.XPath30
        };
        using var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        Assert.Throws<ArgumentNullException>(() => expressionType.SerializeAsV1(writer));
    }

    [Fact]
    public void SerializeAsV1_WithoutVersionThrowsException()
    {
        var expressionType = new ArazzoCriterionExpressionType
        {
            Type = ArazzoCriterionExpressionTypeType.JsonPath
        };
        using var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        Assert.Throws<ArgumentNullException>(() => expressionType.SerializeAsV1(writer));
    }
}
