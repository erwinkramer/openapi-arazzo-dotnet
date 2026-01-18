using BinkyLabs.OpenApi.Arazzo.Writers;

using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Represents a workflow definition in an Arazzo document.
/// </summary>
public class ArazzoWorkflow : IArazzoSerializable, IArazzoExtensible
{
    /// <summary>
    /// Gets or sets the workflow identifier.
    /// </summary>
    public string? WorkflowId { get; set; }

    /// <summary>
    /// Gets or sets the summary of the workflow.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the inputs schema.
    /// </summary>
    public OpenApiSchema? Inputs { get; set; }

    /// <summary>
    /// Gets or sets the set of workflow identifiers that this workflow depends on.
    /// </summary>
    public ISet<string>? DependsOn { get; set; }

    /// <summary>
    /// Gets or sets the list of steps in the workflow.
    /// </summary>
    public IList<ArazzoStep>? Steps { get; set; }

    /// <summary>
    /// Gets or sets the list of success actions.
    /// </summary>
    public IList<IArazzoSuccessAction>? SuccessActions { get; set; }

    /// <summary>
    /// Gets or sets the list of failure actions.
    /// </summary>
    public IList<IArazzoFailureAction>? FailureActions { get; set; }

    /// <summary>
    /// Gets or sets the outputs dictionary.
    /// </summary>
    // TODO: Implement validation during serialization/deserialization that any of the keys 
    // of the Outputs dictionary must match the following regex: ^[a-zA-Z0-9\.\-_]+$
    public IDictionary<string, string>? Outputs { get; set; }

    /// <summary>
    /// Gets or sets the parameters dictionary.
    /// </summary>
    public IDictionary<string, IArazzoParameter>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the extensions dictionary.
    /// </summary>
    public IDictionary<string, IArazzoExtension>? Extensions { get; set; }

    /// <summary>
    /// Serializes the workflow as an OpenAPI Arazzo v1.0.0 JSON object.
    /// </summary>
    /// <param name="writer">The OpenAPI writer to use for serialization.</param>
    public void SerializeAsV1(IOpenApiWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        ArgumentException.ThrowIfNullOrEmpty(WorkflowId);

        writer.WriteStartObject();
        writer.WriteProperty(ArazzoConstants.ArazzoWorkflowWorkflowId, WorkflowId);
        writer.WriteProperty(ArazzoConstants.ArazzoWorkflowSummary, Summary);

        // Write inputs
        if (Inputs != null)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowInputs);
            Inputs.SerializeAsV32(writer);
        }

        // Write dependsOn
        if (DependsOn != null && DependsOn.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowDependsOn);
            writer.WriteStartArray();
            foreach (var dependency in DependsOn)
            {
                writer.WriteValue(dependency);
            }
            writer.WriteEndArray();
        }

        // Write steps
        if (Steps != null && Steps.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowSteps);
            writer.WriteStartArray();
            foreach (var step in Steps)
            {
                step.SerializeAsV1(writer);
            }
            writer.WriteEndArray();
        }

        // Write success actions
        if (SuccessActions != null && SuccessActions.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowSuccessActions);
            writer.WriteStartArray();
            foreach (var action in SuccessActions)
            {
                action.SerializeAsV1(writer);
            }
            writer.WriteEndArray();
        }

        // Write failure actions
        if (FailureActions != null && FailureActions.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowFailureActions);
            writer.WriteStartArray();
            foreach (var action in FailureActions)
            {
                action.SerializeAsV1(writer);
            }
            writer.WriteEndArray();
        }

        // Write outputs
        if (Outputs != null && Outputs.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowOutputs);
            writer.WriteStartObject();
            foreach (var output in Outputs)
            {
                writer.WritePropertyName(output.Key);
                writer.WriteValue(output.Value);
            }
            writer.WriteEndObject();
        }

        // Write parameters
        if (Parameters != null && Parameters.Count > 0)
        {
            writer.WritePropertyName(ArazzoConstants.ArazzoWorkflowParameters);
            writer.WriteStartObject();
            foreach (var parameter in Parameters)
            {
                writer.WritePropertyName(parameter.Key);
                parameter.Value.SerializeAsV1(writer);
            }
            writer.WriteEndObject();
        }

        writer.WriteArazzoExtensions(Extensions, ArazzoSpecVersion.Arazzo1_0);
        writer.WriteEndObject();
    }
}
