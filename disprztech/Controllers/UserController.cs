using disprztech.Models;
using disprztech.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        private async Task<string> SetPasswordHashAsync(User user) {
            var hasher = new PasswordHasher<object>();
            var hashPass = hasher.HashPassword(null, user.PasswordHash);
            return hashPass;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try { return Ok(await _service.GetAllAsync()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("Id/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try { return Ok(await _service.GetByIdAsync(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> Get(string email)
        {
            try { return Ok(await _service.GetByEmailAsync(email));}
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            try
            {
                user.PasswordHash = await SetPasswordHashAsync(user);
                user.Created = DateTime.Now;
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
                var getExisting = await _service.GetAllAsync();

                foreach (var user in users)
                {
                    user.PasswordHash = await SetPasswordHashAsync(user);

                    // Check if the result of GetByEmailAsync is null instead of treating it as a boolean
                    var existingUser = await _service.GetByEmailAsync(user.Email);
                    if (existingUser != null)
                    {
                        user.Created = existingUser.Created;
                        user.Updated = DateTime.Now;
                    }
                    else
                    {
                        user.Created = DateTime.Now;
                    }
                }
                await _service.BulkInsertAsync(users, "Email");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            try
            {
                user.Updated = DateTime.Now;
                user.PasswordHash = await SetPasswordHashAsync(user);
                await _service.UpdateAsync(user.Email, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
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
