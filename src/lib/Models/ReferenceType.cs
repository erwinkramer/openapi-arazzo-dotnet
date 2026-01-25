using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// The type of reference
/// </summary>
public enum ReferenceType
{
    /// <summary>
    /// Success Action component type
    /// </summary>
    [Display("successActions")]
    SuccessAction,
    /// <summary>
    /// Failure Action component type
    /// </summary>
    [Display("failureActions")]
    FailureAction,
    /// <summary>
    /// Parameter component type
    /// </summary>
    [Display("parameters")]
    Parameter,
    /// <summary>
    /// Input component type
    /// </summary>
    [Display("inputs")]
    Input,

}