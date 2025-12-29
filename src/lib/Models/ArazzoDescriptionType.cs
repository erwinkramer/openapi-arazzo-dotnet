using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents the type of Arazzo source description.
/// </summary>
public enum ArazzoDescriptionType
{
    /// <summary>
    /// OpenAPI specification type.
    /// </summary>
    [Display("openapi")]
    OpenAPI,

    /// <summary>
    /// Arazzo specification type.
    /// </summary>
    [Display("arazzo")]
    Arazzo
}