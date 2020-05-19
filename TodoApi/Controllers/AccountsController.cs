using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GMAPI.Models;
using Microsoft.AspNetCore.Authorization;
using GMAPI.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace GMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly PostgresDatabaseContext _context;
        private readonly IMapper _mapper;

        public AccountsController(IMapper mapper, PostgresDatabaseContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts()
        { 
            return await _context.Accounts.ProjectTo<AccountDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return _mapper.Map<AccountDto>(account);
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(Guid id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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


        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }

        private bool AccountExists(Guid id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
