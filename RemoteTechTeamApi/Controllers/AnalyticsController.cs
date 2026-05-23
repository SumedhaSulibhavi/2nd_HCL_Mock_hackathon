using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HypeHealth.API.Data;

namespace HypeHealth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("team-summary")]
        public async Task<IActionResult> TeamSummary()
        {
            var summary = await _context.DailyMoodLogs
                .AsNoTracking()
                .GroupBy(x => x.LogDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgMood = g.Average(x => x.Score),
                    TotalResponses = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return Ok(summary);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> DashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();

            var totalKudos = await _context.KudosCards.CountAsync();

            var todayMoodEntries = await _context.DailyMoodLogs
                .CountAsync(x => x.LogDate.Date == DateTime.UtcNow.Date);

            return Ok(new
            {
                totalUsers,
                totalKudos,
                todayMoodEntries
            });
        }
    }
}