using GMAPI.Models;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public class AccountRepository : IAccountRepository
    {
        PostgresDatabaseContext _context;
        public AccountRepository(PostgresDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccount(Guid id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account> GetFullAccount(Guid id)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Image)
                .Include(a => a.Jobs).ThenInclude(j => j.Company).ThenInclude(c => c.Image)
                .Include(a => a.Jobs).ThenInclude(j => j.Role)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async  Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
