using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace wf_core_sample.Workflows.Steps
{
    public class ProcessInitialDocument : StepBody
    {
        public string Document { get; set; }

        public string WorkflowId { get; set; }

        public string Status { get; set; }

        private readonly WorkFlowEvents _events;

        public ProcessInitialDocument(WorkFlowEvents events)
        {
            _events = events;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _events.Add($"--------------------------------------------");
            _events.Add($"Получен: {Document}");

            if (Document.ToLower() == "doc")
            {
                WorkflowId = context.Workflow.Id;
                Status = "Ожидается ИОП";

                _events.Add($" -- Инициирован документооборот");
            }
            else
            {
                Status = "error";
                _events.Add($" -- Ошибка, неверный тип документа: {Document}.");
            }

            _events.Add($" -- Status: {Status}.");

            return ExecutionResult.Next();
        }
    }
}
