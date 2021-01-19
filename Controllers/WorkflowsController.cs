using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Services.DefinitionStorage;

namespace wf_core_sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : Controller
    {
        private readonly IWorkflowController _workflowService;
        private readonly IWorkflowRegistry _registry;
        private readonly IDefinitionLoader _definitionLoader;
       
        private readonly WorkFlowEvents _events;

        public WorkflowsController(
            IWorkflowController workflowService,
            IWorkflowRegistry registry,
            WorkFlowEvents events, IDefinitionLoader definitionLoader)
        {
            _workflowService = workflowService;
            _events = events;
            _definitionLoader = definitionLoader;
            _registry = registry;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, int? version, string doc)
        {
            var def = _registry.GetDefinition(id, version);
            if (def == null)
                return BadRequest($"Workflow definition {id} for version {version} not found");

            var workflowId = await _workflowService.StartWorkflow(id, version, data: doc);

            return Ok(workflowId);
        }

        [HttpGet("print")]
        public IEnumerable<string> PrintWorkflowEvents() => _events.Get();


        [HttpPost("uploadDefinition")]
        public IActionResult UploadWorkflowDefinition(IFormFile file)
        {
            var definition = new StreamReader(file.OpenReadStream()).ReadToEnd();

            var deserializer = file.FileName.EndsWith("yaml", StringComparison.CurrentCultureIgnoreCase)
                ? Deserializers.Yaml
                : Deserializers.Json;

            var id = _definitionLoader.LoadDefinition(definition, deserializer).Id;

            return Ok(id);
        }
    }
}
