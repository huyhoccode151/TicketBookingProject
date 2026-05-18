using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;
using TicketBookingProject.Server.DTOs;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UiActionController : ControllerBase
    {
        private readonly IUiActionService _UiActionService;
        public UiActionController(IUiActionService UiActionService)
        {
            _UiActionService = UiActionService;
        }
        // GET api/uiaction
        // get all ui action with manage
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _UiActionService.GetAllAsync();
            return items.ToActionResult();
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllUiActions([FromQuery] ListUIActionRequest request)
        {
            var result = await _UiActionService.GetAllUIActions(request);
            return result.ToActionResult();
        }

        // GET api/uiaction/nav  — dùng cho Angular sidebar
        [HttpGet("nav")]
        public async Task<IActionResult> GetNavItems()
        {
            var items = await _UiActionService.GetNavItemsAsync();
            return Ok(items);
        }

        // GET api/uiaction/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _UiActionService.GetByIdAsync(id);
            return item.ToActionResult();
        }

        // POST api/uiaction
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UIActionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _UiActionService.CreateAsync(request);
            return created.ToActionResult();
        }

        // PUT api/uiaction/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UIActionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _UiActionService.UpdateAsync(id, request);
            return updated.ToActionResult();
        }

        // DELETE api/uiaction/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _UiActionService.DeleteAsync(id);
            return result.ToActionResult();
        }
    }
}
