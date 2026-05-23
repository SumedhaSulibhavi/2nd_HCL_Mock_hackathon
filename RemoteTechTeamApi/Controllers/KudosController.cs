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
    public class KudosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KudosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.KudosCards
                .AsNoTracking()
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .OrderByDescending(x => x.Timestamp)
                .Take(50)
                .Select(x => new
                {
                    x.Id,
                    Sender = x.Sender.Username,
                    Receiver = x.Receiver.Username,
                    x.Message,
                    x.Timestamp
                })
                .ToListAsync();

            return Ok(data);
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendKudos(CreateKudosDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (senderId == dto.ReceiverId)
            {
                return BadRequest("You cannot send kudos to yourself");
            }

            var kudos = new KudosCard
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                Timestamp = DateTime.UtcNow
            };

            _context.KudosCards.Add(kudos);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kudos sent successfully"
            });
        }
    }
}