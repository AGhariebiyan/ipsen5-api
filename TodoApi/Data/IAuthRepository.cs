using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public interface IAuthRepository
    {
        Task<Account> Register(Account Account, string Password);
        Task<Account> Login(string Email, string Password);
        Task<bool> AccountExists(string Email);
    }
}
