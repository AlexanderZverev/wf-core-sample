using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wf_core_sample.Workflows;
using WorkflowCore.Interface;

namespace wf_core_sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        private readonly IWorkflowController _workflowService;

        public EventsController(IWorkflowController workflowService)
        {
            _workflowService = workflowService;
        }

        [HttpPost("{eventName}/{eventKey}")]
        public async Task<IActionResult> Post(string eventName, string eventKey, [FromBody] UntypedDocumentWorkflowData eventData)
        {
            await _workflowService.PublishEvent(eventName, eventKey, eventData.Document);
            return Ok();
        }
    }
}
