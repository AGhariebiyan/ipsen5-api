using GMAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public interface ICompanyRepository
    {
        Task<Company> GetCompany(Guid  id);
        Task<IEnumerable<Company>> GetCompanies();
        Task<Company> CreateCompany(Company comp);

        Task<bool> DeleteCompany(Guid id);

        Task<Company> UpdateCompany(Guid id, Company company);
        Task<bool> CanEditCompany(Guid companyId, Guid accountId);

        Task<bool> SaveAll();
    }
}
