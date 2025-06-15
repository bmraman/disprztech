using disprztech.Models;
using disprztech.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace disprztech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<User> _service;

        public UserController(IService<User> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try { return Ok(await _service.GetAllAsync()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try { return Ok(await _service.GetByIdAsync(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            try
            {
                await _service.CreateAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("BulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<User> users)
        {
            try
            {
                await _service.BulkInsertAsync(users, "Email");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            try
            {
                await _service.UpdateAsync(id, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
