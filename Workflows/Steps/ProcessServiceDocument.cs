using System;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace wf_core_sample.Workflows.Steps
{
    public class ProcessServiceDocument : StepBody
    {
        private readonly WorkFlowEvents _events;

        public string Document { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public ProcessServiceDocument(WorkFlowEvents events)
        {
            _events = events;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _events.Add($"Получен: {Document}");

            var expectedDocType = Type.Split(" ").Select(d => d.ToLower()).ToArray();
            var receivedDocument = Document.ToLower();

            if (receivedDocument == "iop" && expectedDocType.Contains(receivedDocument))
            {
                _events.Add(" -- Документ доставлен. Ожидается подтверждение или отказ.");
                Status = "Документ доставлен получателю.";
            }
            else if (receivedDocument == "uou" && expectedDocType.Contains(receivedDocument))
            {
                _events.Add(" -- Получатель отказал по документу.");
                Status = "Отказано";
            }
            else if (receivedDocument == "ok" && expectedDocType.Contains(receivedDocument))
            {
                _events.Add(" -- Получатель подтвердил получение документа.");
                Status = "Документ успешно передан";
            }
            else
            {
                _events.Add($" -- Неверный тип документа {Document}. Допустимые типы: {Type}");
                Status = "error";
            }

            _events.Add($" -- Status: {Status}");

            return ExecutionResult.Next();
        }
    }
}
