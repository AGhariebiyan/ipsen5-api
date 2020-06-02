using GMAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PostgresDatabaseContext _context;

        public AuthRepository(PostgresDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Account> Login(string email, string password)
        {
            var Account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(x => x.Email == email);

            if (Account == null) {
                return null;
            }

            if (!VerfiyPasswordHash(password,  Account.PasswordHash, Account.PasswordSalt)) {
                return null;
            }

            return Account;

        }

        private bool VerfiyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<Account> Register(Account account, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            account.PasswordHash = passwordHash;
            account.PasswordSalt = passwordSalt;

            PermissionRole nonMemberAccount = await _context.PermissionRoles.FirstOrDefaultAsync(p => p.InternalName == "non-member");

            account.RoleId = nonMemberAccount.Id;

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return account;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> AccountExists(string Email)
        {
            if (await _context.Accounts.AnyAsync(x => x.Email == Email)){
                return true;
            }
            return false;
        }
    }
}
