using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HypeHealth.API.Data;
using HypeHealth.API.DTOs;
using HypeHealth.API.Models;

namespace HypeHealth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveyController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitMood(MoodSubmissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var today = DateTime.UtcNow.Date;

            var alreadyLogged = await _context.DailyMoodLogs
                .AnyAsync(x => x.UserId == userId &&
                               x.LogDate.Date == today);

            if (alreadyLogged)
            {
                return BadRequest("Mood already logged for today");
            }

            var mood = new DailyMoodLog
            {
                UserId = userId,
                Score = dto.Score,
                LogDate = DateTime.UtcNow
            };

            _context.DailyMoodLogs.Add(mood);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Mood logged successfully"
            });
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLogs()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var logs = await _context.DailyMoodLogs
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.LogDate)
                .ToListAsync();

            return Ok(logs);
        }
    }
}