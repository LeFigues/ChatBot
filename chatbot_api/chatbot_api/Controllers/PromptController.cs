using chatbot_api.Model;
using chatbot_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly PromptService _promptService;

        public PromptController(PromptService promptService)
        {
            _promptService = promptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _promptService.GetAllAsync());

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var prompt = await _promptService.GetByKeyAsync(key);
            return prompt is null ? NotFound() : Ok(prompt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromptDefinition prompt)
        {
            await _promptService.CreateAsync(prompt);
            return CreatedAtAction(nameof(GetByKey), new { key = prompt.Key }, prompt);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] PromptDefinition updated)
        {
            var existing = await _promptService.GetByKeyAsync(key);
            if (existing is null) return NotFound();

            updated.Id = existing.Id;
            await _promptService.UpdateAsync(key, updated);
            return NoContent();
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var prompt = await _promptService.GetByKeyAsync(key);
            if (prompt is null) return NotFound();

            await _promptService.DeleteAsync(key);
            return NoContent();
        }
    }
}
