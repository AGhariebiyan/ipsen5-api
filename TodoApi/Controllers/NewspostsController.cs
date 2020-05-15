using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GMAPI.Models;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewspostsController : ControllerBase
    {
        private readonly PostgresDatabaseContext _context;

        public NewspostsController(PostgresDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Newsposts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Newspost>>> GetNewspost()
        {
            return await _context.Newspost.ToListAsync();
        }

        // GET: api/Newsposts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Newspost>> GetNewspost(int id)
        {
            var newspost = await _context.Newspost.FindAsync(id);

            if (newspost == null)
            {
                return NotFound();
            }

            return newspost;
        }

        // PUT: api/Newsposts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewspost(int id, Newspost newspost)
        {
            if (id != newspost.NewspostId)
            {
                return BadRequest();
            }

            _context.Entry(newspost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewspostExists(id))
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

        // POST: api/Newsposts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Newspost>> PostNewspost(Newspost newspost)
        {
            _context.Newspost.Add(newspost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNewspost", new { id = newspost.NewspostId }, newspost);
        }

        // DELETE: api/Newsposts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Newspost>> DeleteNewspost(int id)
        {
            var newspost = await _context.Newspost.FindAsync(id);
            if (newspost == null)
            {
                return NotFound();
            }

            _context.Newspost.Remove(newspost);
            await _context.SaveChangesAsync();

            return newspost;
        }

        private bool NewspostExists(int id)
        {
            return _context.Newspost.Any(e => e.NewspostId == id);
        }
    }
}
