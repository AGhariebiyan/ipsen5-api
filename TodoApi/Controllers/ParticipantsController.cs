using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GMAPI.Models;
using Microsoft.Extensions.Logging;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly PostgresDatabaseContext _context;

        public ParticipantsController(PostgresDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipant()
        {
            return await _context.Participant.ToListAsync();
        }

        [HttpGet("getForEvent/{eventId}")]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipantsForEvent(Guid eventId)
        {
           return await _context.Participant.Where(x => x.EventId == eventId).ToListAsync();
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(Guid id)
        {
            var participant = await _context.Participant.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // PUT: api/Participants/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(Guid id, Participant participant)
        {
            if (id != participant.Id)
            {
                return BadRequest();
            }

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Participants
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(Participant participant)
        {
            _context.Participant.Add(participant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParticipant", new { id = participant.Id }, participant);
        }

        // DELETE: api/Participants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Participant>> DeleteParticipant(Guid id)
        {
            var participant = await _context.Participant.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            _context.Participant.Remove(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        private bool ParticipantExists(Guid id)
        {
            return _context.Participant.Any(e => e.Id == id);
        }
    }
}
