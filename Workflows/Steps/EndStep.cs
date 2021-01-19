using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace wf_core_sample.Workflows.Steps
{
    public class EndStep : StepBody
    {
        public string Status { get; set; }

        private readonly WorkFlowEvents _events;

        public EndStep(WorkFlowEvents events)
        {
            _events = events;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _events.Add($"Документооборот завершен.");
            _events.Add($" -- Status: {Status}");
            _events.Add($"--------------------------------------------");

            return ExecutionResult.Next();
        }
    }
}
