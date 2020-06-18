using AutoMapper;
using GMAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Data
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly PostgresDatabaseContext _context;
        private readonly IMapper _mapper;
        public CompanyRepository(PostgresDatabaseContext context,
                                 IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CanEditCompany(Guid companyId, Guid accountId)
        {
            WorksAt worksAt = await _context.WorksAt.Include(wa => wa.Role).FirstOrDefaultAsync(wa =>
                 wa.CompanyId == companyId && wa.AccountId == accountId && wa.Role.CanEditCompany == true);
            return worksAt != null;
        }

        public async Task<Company> CreateCompany(Company comp)
        {
            await _context.Companies.AddAsync(comp);
            await SaveAll();
            return comp;
        }

        public async Task<bool> DeleteCompany(Guid id)
        {
            var compFromDb = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (compFromDb == null)
            {
                return false;
            }
            else {
                _context.Companies.Remove(compFromDb);
                return await SaveAll();
            }
        }

        public async  Task<IEnumerable<Company>> GetCompanies()
        {
            var companiesToReturn = await _context.Companies.Include(c => c.Image).ToListAsync();
            return companiesToReturn;
        }

        public async Task<Company> GetCompany(Guid id)
        {
            var companyToReturn = await _context.Companies.Include(c => c.Image).FirstOrDefaultAsync(c => c.Id == id);
            return companyToReturn;
        }

        public async Task<bool> SaveAll()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<Company> UpdateCompany(Guid id, Company company)
        {
            if (company.Id != id) {
                return null;
            }

            var companyFromRepo = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

            if (companyFromRepo == null) {
                return null;
            }

            var updatedCompany = company;

            if (await SaveAll())
            {
                return updatedCompany;
            }

            return null;
        }
    }
}
