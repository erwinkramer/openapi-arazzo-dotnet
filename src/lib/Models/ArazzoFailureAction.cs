using BinkyLabs.OpenApi.Arazzo.Writers;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents a failure action definition.
/// </summary>
public class ArazzoFailureAction : IArazzoFailureAction
{
    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public ArazzoFailureType? Type { get; set; }

    /// <inheritdoc/>
    public string? WorkflowId { get; set; }

    /// <inheritdoc/>
    public string? StepId { get; set; }

    /// <inheritdoc/>
    public decimal? RetryAfter { get; set; }

    /// <inheritdoc/>
    public ulong? RetryLimit { get; set; }

    /// <inheritdoc/>
    public IList<ArazzoCriterion>? Criteria { get; set; }

    /// <inheritdoc/>
    public IDictionary<string, IArazzoExtension>? Extensions { get; set; }

    /// <summary>
    /// Serializes the failure action as an OpenAPI Arazzo v1.0.0 JSON object.
    /// </summary>
    /// <param name="writer">The OpenAPI writer to use for serialization.</param>
    public void SerializeAsV1(IOpenApiWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        ArgumentException.ThrowIfNullOrEmpty(Name);
        if (!Type.HasValue)
        {
            throw new ArgumentNullException(nameof(Type));
        }

        writer.WriteStartObject();
        writer.WriteRequiredProperty(ArazzoConstants.ArazzoFailureActionName, Name);
        writer.WriteRequiredProperty(ArazzoConstants.ArazzoFailureActionType, Type.Value.GetDisplayName());
        if (!string.IsNullOrEmpty(WorkflowId))
        {
            writer.WriteProperty(ArazzoConstants.ArazzoFailureActionWorkflowId, WorkflowId);
        }
        if (!string.IsNullOrEmpty(StepId))
        {
            writer.WriteProperty(ArazzoConstants.ArazzoFailureActionStepId, StepId);
        }
        if (RetryAfter.HasValue)
        {
            writer.WriteProperty(ArazzoConstants.ArazzoFailureActionRetryAfter, RetryAfter.Value);
        }
        if (RetryLimit.HasValue)
        {
            writer.WriteProperty(ArazzoConstants.ArazzoFailureActionRetryLimit, (long)RetryLimit.Value);
        }
        if (Criteria != null && Criteria.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoFailureActionCriteria);
            writer.WriteStartArray();
            foreach (var criterion in Criteria)
            {
                criterion.SerializeAsV1(writer);
            }
            writer.WriteEndArray();
        }
        writer.WriteArazzoExtensions(Extensions, ArazzoSpecVersion.Arazzo1_0);
        writer.WriteEndObject();
    }
}