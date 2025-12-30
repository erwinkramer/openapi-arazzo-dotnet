using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents the version of a criterion expression.
/// </summary>
public enum ArazzoCriterionExpressionVersion
{
    /// <summary>
    /// draft-goessner-dispatch-jsonpath-00 version.
    /// </summary>
    [Display("draft-goessner-dispatch-jsonpath-00")]
    DraftGoessnerDispatchJsonPath00,

    /// <summary>
    /// XPath 3.0 version.
    /// </summary>
    [Display("xpath-30")]
    XPath30,

    /// <summary>
    /// XPath 2.0 version.
    /// </summary>
    [Display("xpath-20")]
    XPath20,

    /// <summary>
    /// XPath 1.0 version.
    /// </summary>
    [Display("xpath-10")]
    XPath10
}
