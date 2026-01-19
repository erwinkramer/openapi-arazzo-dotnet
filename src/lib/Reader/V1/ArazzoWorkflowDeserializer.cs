using System.Text;

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace BinkyLabs.OpenApi.Arazzo.Reader.V1;

internal static partial class ArazzoV1Deserializer
{
    public static readonly FixedFieldMap<ArazzoWorkflow> WorkflowFixedFields = new()
    {
        { ArazzoConstants.ArazzoWorkflowWorkflowId, static (o, v) => o.WorkflowId = v.GetScalarValue() },
        { ArazzoConstants.ArazzoWorkflowSummary, static (o, v) => o.Summary = v.GetScalarValue() },
        { ArazzoConstants.ArazzoWorkflowInputs, static (o, v) => o.Inputs = LoadSchema(v) },
        { ArazzoConstants.ArazzoWorkflowDependsOn, static (o, v) =>
        {
            var list = v.CreateSimpleList(static n => n.GetScalarValue());
            if (list != null && list.Count > 0)
            {
                o.DependsOn = new HashSet<string>(list.Where(static s => s != null).Cast<string>());
            }
        } },
        { ArazzoConstants.ArazzoWorkflowSteps, static (o, v) => o.Steps = v.CreateList(LoadStep) },
        { ArazzoConstants.ArazzoWorkflowSuccessActions, static (o, v) => o.SuccessActions = v.CreateList<IArazzoSuccessAction>(LoadSuccessAction) },
        { ArazzoConstants.ArazzoWorkflowFailureActions, static (o, v) => o.FailureActions = v.CreateList<IArazzoFailureAction>(LoadFailureAction) },
        { ArazzoConstants.ArazzoWorkflowOutputs, static (o, v) => o.Outputs = v.CreateSimpleMap(static n => n.GetScalarValue()) },
        { ArazzoConstants.ArazzoWorkflowParameters, static (o, v) => o.Parameters = v.CreateMap<IArazzoParameter>(LoadParameter) },
    };

    public static readonly PatternFieldMap<ArazzoWorkflow> WorkflowPatternFields = new()
    {
        { s => s.StartsWith(ArazzoConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n) => o.AddExtension(k, LoadExtension(k, n)) }
    };

    public static ArazzoWorkflow LoadWorkflow(ParseNode node)
    {
        var mapNode = node.CheckMapNode("Workflow");
        var workflow = new ArazzoWorkflow();

        // TODO: Implement validation during serialization/deserialization that any of the keys 
        // of Outputs and Parameters dictionaries must match the following regex: ^[a-zA-Z0-9\.\-_]+$

        ParseMap(mapNode, workflow, WorkflowFixedFields, WorkflowPatternFields);

        return workflow;
    }
}