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
using GMAPI.Other;
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
        // GET: api/Accounts/admin
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<AccountForMeDto>>> GetAccountsForAdmin()
        {
            var accountsFromRepo = await _context.Accounts
                .Include(a => a.Jobs).ThenInclude(j => j.Role)
                .Include(a => a.Jobs).ThenInclude(j => j.Company).ThenInclude(c => c.Image)
                .ProjectTo<AccountForMeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return accountsFromRepo;
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
            var account = await _context.Accounts
                .Include(a => a.Image)
                .Include(a=>a.Jobs).ThenInclude(j => j.Company)
                .Include(a => a.Jobs).ThenInclude(j => j.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

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
        
        [HttpGet("{Id}/jobrequests")]
        public async Task<ActionResult<WorksAt[]>> GetJobRequests(Guid Id)
        {
            var JobRequests = await _context.WorksAt.Where(w => w.CompanyId == Id)
                .Where(w => w.Accepted == false).Include(w => w.Account)
                .Include(w => w.Company).Include(w => w.Role).IgnoreQueryFilters<WorksAt>().ToListAsync();
            return Ok(JobRequests);
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

            Verification instance = await _context.Verifications.FirstOrDefaultAsync(p => p.AccountId == id);
            if (instance != null) _context.Verifications.Remove(instance);
            await _context.SaveChangesAsync();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }

        // PUT: api/Accounts/5/password
        [HttpPut("{id}/password")]
        public async Task<ActionResult> ChangePassword(Guid id, AccountForPasswordDto changedAccount)
        {
            if (id != changedAccount.Id)
            {
                return BadRequest();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            PasswordService.CreatePasswordHash(changedAccount.Password, out var passwordHash, out var passwordSalt);

            account.PasswordHash = passwordHash;
            account.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() {
            var roles = await _context.PermissionRoles.ToListAsync();
            return Ok(roles);
        }
        
        [HttpPut("{id}/roles/{roleId}")]
        public async Task<IActionResult> UpdateUserRole(Guid id, Guid roleId) {
            PermissionRole role = await _context.PermissionRoles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null) {
                return BadRequest("Role doesn't exist");
            }
            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) {
                return BadRequest("User not found");
            }

            account.RoleId = role.Id;
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }
            else {
                return BadRequest("Could not update role");
            }
        
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
                var Role = worksAt.Role;
                worksAt.Role.CanEditCompany = true;
                await _context.Role.AddAsync(Role);
                await _context.SaveChangesAsync();
                worksAt.Role.Id = Role.Id;
            }

            worksAt.AccountId = id;

            userToUpdate.Jobs.Add(worksAt);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(worksAt);
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

        [HttpPut("jobs/{jobId}")]
        public async Task<IActionResult> AcceptJobRequest(Guid jobId, WorksAt worksAt)
        {

            if(jobId != worksAt.Id)
            {
                return BadRequest("Id's do not match");
            }

            var JobFromRepo = await _context.WorksAt.Where(w => w.Id == worksAt.Id)
                .Where(w => w.AccountId == worksAt.AccountId)
                .IgnoreQueryFilters().SingleAsync();
            JobFromRepo.Accepted = true;

            await _context.SaveChangesAsync();

            return Ok(JobFromRepo);

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


        [HttpDelete("jobs/{jobId}")]
         public async Task<IActionResult> DenyJob(Guid jobId)
         {

            var worksAtToRemove = await _context.WorksAt.IgnoreQueryFilters().FirstOrDefaultAsync(wa => wa.Id == jobId);
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
