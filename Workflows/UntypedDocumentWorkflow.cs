using System;
using wf_core_sample.Workflows.Steps;
using WorkflowCore.Interface;

namespace wf_core_sample.Workflows
{
    public class UntypedDocumentWorkflow : IWorkflow<UntypedDocumentWorkflowData>
    {
        public string Id => "UntypedDocumentWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<UntypedDocumentWorkflowData> builder)
        {
            builder
                .StartWith<ProcessInitialDocument>()
                    .Input(step => step.Document, data => data.Document)
                    .Output(data => data.WorkflowId, step => step.WorkflowId)
                    .Output(data => data.Status, step => step.Status)
                .If(data => data.Status.ToLower() == "error").Do(then => then
                    .StartWith<EndStep>()
                    .Input(step => step.Status, data => data.Status)
                    .EndWorkflow()
                )
                .WaitFor("serviceDoc", data => data.WorkflowId)
                    .Output(data => data.Document, step => step.EventData)
                .Then<ProcessServiceDocument>()
                    .Input(step => step.Type, _ => "iop")
                    .Input(step => step.Document, data => data.Document)
                    .Output(data => data.Status, step => step.Status)
                .If(data => data.Status.ToLower() == "error").Do(then => then
                    .StartWith<EndStep>()
                    .Input(step => step.Status, data => data.Status)
                    .EndWorkflow()
                )
                .WaitFor("serviceDoc", data => data.WorkflowId, data => DateTime.Now)
                    .Output(data => data.Document, step => step.EventData)
                .Then<ProcessServiceDocument>()
                    .Input(step => step.Type, _ => "UOU OK")
                    .Input(step => step.Document, data => data.Document)
                    .Output(data => data.Status, step => step.Status)
                .Then<EndStep>()
                    .Input(step => step.Status, data => data.Status)
                    .EndWorkflow();
        }
    }

    public class UntypedDocumentWorkflowData
    {
        public string WorkflowId { get; set; }

        public string Document { get; set; }

        public string Status { get; set; }
    }
}
