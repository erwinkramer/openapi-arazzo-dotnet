using System.ComponentModel.DataAnnotations;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents the type of Arazzo source description.
/// </summary>
public enum ArazzoDescriptionType
{
    /// <summary>
    /// OpenAPI specification type.
    /// </summary>
    [Display(Name = "openapi")]
    OpenAPI,

    /// <summary>
    /// Arazzo specification type.
    /// </summary>
    [Display(Name = "arazzo")]
    Arazzo
}