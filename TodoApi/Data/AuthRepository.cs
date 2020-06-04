﻿using GMAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using GMAPI.Other;
using Microsoft.AspNetCore.Mvc;

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

            var createdAccount = await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return createdAccount.Entity;
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

        public async Task<Account> VerifyEmail(Guid verificationId)
        {
            var verification = await _context.Verifications.Include(v => v.Account).FirstOrDefaultAsync(x => x.Id == verificationId);
            if (verification == null)
            {
                return null;
            }

            var account = verification.Account;
            _context.Verifications.Remove(verification);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Verification> CreateVerificationInstance(Verification verification)
        {
            var ver = await _context.Verifications.AddAsync(verification);
            await _context.SaveChangesAsync();
            return ver.Entity;
        }    
        
        public async Task<ActionResult<IEnumerable<Verification>>> GetVerifications()
        {
            return await _context.Verifications.Include(v => v.Account).ToListAsync();
        }    
    }
}
