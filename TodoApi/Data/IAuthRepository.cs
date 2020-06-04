using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GMAPI.Data
{
    public interface IAuthRepository
    {
        Task<Account> Register(Account Account, string Password);
        Task<Account> Login(string Email, string Password);
        Task<bool> AccountExists(string Email);
        Task<Account> VerifyEmail(Guid verificationId);
        Task<Verification> CreateVerificationInstance(Verification verification);

        Task<ActionResult<IEnumerable<Verification>>> GetVerifications();
    }
}
