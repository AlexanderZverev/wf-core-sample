using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using wf_core_sample.Workflows;
using WorkflowCore.Interface;
using WorkflowCore.Services.DefinitionStorage;

namespace wf_core_sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : Controller
    {
        private readonly IWorkflowController _workflowService;
        private readonly IPersistenceProvider _workflowStore;
        private readonly IWorkflowRegistry _registry;
        private readonly IDefinitionLoader _definitionLoader;
        private readonly ISearchIndex _searchService;

        private readonly WorkFlowEvents _events;

        public WorkflowsController(
            IWorkflowController workflowService,
            IWorkflowRegistry registry,
            WorkFlowEvents events, IDefinitionLoader definitionLoader,
            IPersistenceProvider workflowStore, ISearchIndex searchService)
        {
            _workflowService = workflowService;
            _events = events;
            _definitionLoader = definitionLoader;
            _workflowStore = workflowStore;
            _searchService = searchService;
            _registry = registry;
        }

        /// <summary>
        /// Начать документооборот
        /// </summary>
        /// <param name="workflowName">Название документооборота</param>
        /// <param name="version">Версия</param>
        /// <param name="document">Документ ('doc')</param>
        /// <returns></returns>
        [HttpPost("{workflowName}")]
        public async Task<IActionResult> Post(string workflowName, int? version, string document)
        {
            var def = _registry.GetDefinition(workflowName, version);

            if (def == null)
            {
                return BadRequest($"Workflow definition {workflowName} for version {version} not found");
            }

            var workflowId = await _workflowService.StartWorkflow(
                workflowName, 
                version, 
                data: new UntypedDocumentWorkflowData { Document = document });

            return Ok(workflowId);
        }

        /// <summary>
        /// Отобразить события документооборотов.
        /// </summary>
        /// <returns></returns>
        [HttpGet("print")]
        public IEnumerable<string> PrintWorkflowEvents() => _events.Get();

        /// <summary>
        /// Загрузить текстовое описание документооборота
        /// </summary>
        /// <param name="file">Файл с описанием</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Получить информацию по документообороту
        /// </summary>
        /// <param name="id">ИД документооборота</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _workflowStore.GetWorkflowInstance(id);
            return Json(result);
        }

    }
}
