namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents a failure action definition.
/// </summary>
public interface IArazzoFailureAction : IArazzoSerializable, IArazzoExtensible, IArazzoReferenceable
{
    /// <summary>
    /// Gets or sets the failure action name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets or sets the type of the failure action.
    /// </summary>
    ArazzoFailureType? Type { get; }

    /// <summary>
    /// Gets or sets the workflow identifier.
    /// </summary>
    string? WorkflowId { get; }

    /// <summary>
    /// Gets or sets the step identifier.
    /// </summary>
    string? StepId { get; }

    /// <summary>
    /// Gets or sets the retry after time in seconds.
    /// </summary>
    decimal? RetryAfter { get; }

    /// <summary>
    /// Gets or sets the retry limit.
    /// </summary>
    ulong? RetryLimit { get; }

    /// <summary>
    /// Gets or sets the criteria list.
    /// </summary>
    IList<ArazzoCriterion>? Criteria { get; }
}