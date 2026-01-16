using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents the type of a failure action.
/// </summary>
public enum ArazzoFailureType
{
    /// <summary>
    /// End failure action type.
    /// </summary>
    [Display("end")]
    End,

    /// <summary>
    /// Retry failure action type.
    /// </summary>
    [Display("retry")]
    Retry,

    /// <summary>
    /// Goto failure action type.
    /// </summary>
    [Display("goto")]
    Goto
}