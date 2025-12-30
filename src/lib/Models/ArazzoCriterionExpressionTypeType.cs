using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents the type of a criterion expression.
/// </summary>
public enum ArazzoCriterionExpressionTypeType
{
    /// <summary>
    /// Simple criterion expression type.
    /// </summary>
    [Display("simple")]
    Simple,
    /// <summary>
    /// Regular expression criterion expression type.
    /// </summary>
    [Display("regex")]
    Regex,
    /// <summary>
    /// JSONPath criterion expression type.
    /// </summary>
    [Display("jsonpath")]
    JsonPath,

    /// <summary>
    /// XPath criterion expression type.
    /// </summary>
    [Display("xpath")]
    XPath,
}