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

        /// <summary>
        /// Отправить документ по запущенному документообороту
        /// </summary>
        /// <param name="eventName">Имя события которое ожидает запущщенный документооборот</param>
        /// <param name="eventKey">ИД документооборота</param>
        /// <param name="doc">Документ ('ok, iop, uou')</param>
        /// <returns></returns>
        [HttpPost("{eventName}/{eventKey}")]
        public async Task<IActionResult> Post(string eventName, string eventKey, string doc)
        {
            await _workflowService.PublishEvent(eventName, eventKey, doc);
            return Ok();
        }
    }
}
