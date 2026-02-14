using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KhoaCNTT.Application.Interfaces.Services;

namespace KhoaCNTT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ISchoolApiService _schoolApi;
        public StudentsController(ISchoolApiService schoolApi)
        {
            _schoolApi = schoolApi;
        }

        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Chưa đăng nhập.");
            }

            try
            {
                var data = await _schoolApi.GetScoresAsync(token);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule(string semesterId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Chưa đăng nhập.");
            }
            try
            {
                var data = await _schoolApi.GetScheduleAsync(token, semesterId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
