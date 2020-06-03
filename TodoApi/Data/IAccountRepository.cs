using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public interface IAccountRepository
    {
        Task<Account> GetFullAccount(Guid id);
        Task<Account> GetAccount(Guid id);
    }
}
