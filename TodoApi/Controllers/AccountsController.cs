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

namespace GMAPI.Controllers
{
    [Authorize]
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
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = environment;
            _repo = repo;
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

        [HttpGet("me")]
        public async Task<ActionResult<AccountForMeDto>> GetMyAccount()
        {
            Guid id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                return NotFound();
            }

            return _mapper.Map<AccountForMeDto>(account);
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

        [HttpPost("{id}/picture")]
        public async Task<ActionResult<String>> SetPicture(Guid id, [FromForm] IFormFile picture)
        {
            var existingFile = Directory.EnumerateFiles(_hostingEnvironment.ContentRootPath +"/Images/ProfilePictures").SingleOrDefault(f => f.Contains(id + "."));
            if(existingFile != null) System.IO.File.Delete(existingFile);
            var uploads = Path.Combine("Images/ProfilePictures", id + "." + picture.FileName.Split('.').Last());
            using (var fileStream = new FileStream(uploads, FileMode.Create)) {
                await picture.CopyToAsync(fileStream);
            }
            return uploads;
        }
        
        [HttpGet("{id}/picture")]
        public async Task<ActionResult> GetPicture(Guid id)
        {
            var filename = Directory.EnumerateFiles(_hostingEnvironment.ContentRootPath +"/Images/ProfilePictures").SingleOrDefault(f => f.Contains(id + "."));
            var file = PhysicalFile(filename, "image/jpeg");
            return file;
        }

        private bool AccountExists(Guid id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
