using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GMAPI.Models;
using Microsoft.AspNetCore.Authorization;
using GMAPI.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using GMAPI.Data;
using Microsoft.AspNetCore.Connections.Features;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly PostgresDatabaseContext _context;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _hostingEnvironment;
        private IAccountRepository _repo;

        public AccountsController(IMapper mapper, PostgresDatabaseContext context, IWebHostEnvironment environment, IAccountRepository repo)
        {
            _repo = repo;
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = environment;
            _repo = repo;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts()
        {
            var accountsFromRepo = await _context.Accounts
                .Include(a => a.Jobs).ThenInclude(j => j.Role)
                .Include(a => a.Jobs).ThenInclude(j => j.Company).ThenInclude(c => c.Image)
                .ProjectTo<AccountDto>(_mapper.ConfigurationProvider).ToListAsync();
            
            return accountsFromRepo;
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
        {
            var account = await _context.Accounts.Include(a => a.Image).FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                return NotFound();
            }

            return _mapper.Map<AccountDto>(account);
        }

        [HttpGet("me")]
        public async Task<ActionResult<AccountForMeDto>> GetMyAccount()
        {
            Guid id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var account = await _repo.GetFullAccount(id);

            if (account == null)
            {
                return NotFound();
            }
            var returnAccount = _mapper.Map<AccountForMeDto>(account);

            return Ok(returnAccount);
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(Guid id, AccountForUpdateDto accountForUpdate)
        {
            if (id != accountForUpdate.Id)
            {
                return BadRequest();
            }

            var accountFromRepo = await _repo.GetAccount(id);
                
            _mapper.Map(accountForUpdate, accountFromRepo);

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

        [HttpPut("{id}/Image")]
        public async Task<ActionResult> SetImage(Guid id, Image image)
        {
            var account = _repo.GetAccount(id).Result;
            account.Image = image;
            var changes = await _context.SaveChangesAsync();
            if (changes == 0)
            {
                throw new Exception("something went wrong");
            }
            return Ok();
        }

        private bool AccountExists(Guid id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }


        [HttpPost("{id}/jobs")]
        public async Task<IActionResult> AddJob(Guid id, WorksAtForCreateDto worksAtForCreate) {

            var worksAt = _mapper.Map<WorksAt>(worksAtForCreate);
            
            Guid jwtId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != jwtId) {
                return Unauthorized();
            }

            var userToUpdate = await _context.Accounts.Include(a => a.Jobs).FirstOrDefaultAsync(a => a.Id == id);
            if (userToUpdate == null) {
                return BadRequest("User does not exist");
            }

            
            if (worksAt.Role != null) {
                worksAt.Role.canEditCompany = true;
            }

            worksAt.AccountId = id;

            userToUpdate.Jobs.Add(worksAt);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }
            else {
                return BadRequest("Did not update");
            }
        }
        [HttpPut("{id}/jobs/{jobId}")]
        public async Task<IActionResult> UpdateJobDescription(Guid jobId, WorksAtForUpdateDto worksAtForUpdate) {
            var worksAt = _mapper.Map<WorksAt>(worksAtForUpdate);

            var jobFromRepo = await _context.WorksAt
                .Include(a => a.Account)
                .Include(a => a.Role)
                .Include(a => a.Company)
                .FirstOrDefaultAsync(w => w.Id == jobId);

            Guid jwtId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (jobFromRepo.AccountId != jwtId) {
                return Unauthorized();
            }
            jobFromRepo.Role.Title = worksAt.Role.Title;

            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }
            else {
                return BadRequest("Could not update the job");
            }
        }

        [HttpDelete("{id}/jobs/{jobId}")]
        public async Task<IActionResult> RemoveJob(Guid id, Guid jobId)
        {

            Guid jwtId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != jwtId)
            {
                return Unauthorized();
            }

            var worksAtToRemove = await _context.WorksAt.FirstOrDefaultAsync(wa => wa.Id == jobId);
            if (worksAtToRemove == null)
            {
                return BadRequest("You are not working for this company");
            }

            _context.WorksAt.Remove(worksAtToRemove);

          
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Did not update");
            }

        }

    }
}
