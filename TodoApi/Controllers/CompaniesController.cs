using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using GMAPI.Data;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _repo;
        private readonly IMapper _mapper;
        private PostgresDatabaseContext _context;
        public CompaniesController(ICompanyRepository repo, IMapper mapper, PostgresDatabaseContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(Guid id) {
            var companyFromRepo = await _repo.GetCompany(id);
            if (companyFromRepo == null) {
                return NotFound();
            }

            var compForReturn = _mapper.Map<CompanyForReturnDto>(companyFromRepo);

            return Ok(compForReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies() {
            var companiesFromRepo = await _repo.GetCompanies();

            var companiesForReturn = _mapper.Map<IEnumerable<CompanyForReturnDto>>(companiesFromRepo);

            return Ok(companiesForReturn);
        }

        [HttpGet("{id}/forRegistration")]
        public async Task<IActionResult> GetCompaniesForRegistration(Guid Id)
        {
            
            var Jobs = await _context.WorksAt.Where(w => w.AccountId == Id).Select(w => w.CompanyId).ToListAsync();
            var companiesFromRepo = await _context.Companies.Where(c => !Jobs.Contains(c.Id)).Include(c => c.Image).ToListAsync();
            var companiesForReturn = _mapper.Map<IEnumerable<CompanyForReturnDto>>(companiesFromRepo);

            return Ok(companiesForReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CompanyForCreateDto compForCreate) {
            var comp = _mapper.Map<Company>(compForCreate);
            var companyToCreate = await _repo.CreateCompany(comp);
            return Ok(companyToCreate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCompany(Guid id, CompanyForUpdateDto companyForUpdateDto) {
            Company company = await _repo.GetCompany(id);
            Guid jwtId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != company.Id) {
                return BadRequest("Id's do not match");
            }
            if (company == null) {
                return BadRequest();
            }

            if (! await _repo.CanEditCompany(company.Id, jwtId)) {
                return Unauthorized();
            }
            company = _mapper.Map(companyForUpdateDto, company);
            Company updatedCompany = await _repo.UpdateCompany(company.Id, company);
            if (updatedCompany != null) {
                return Ok(_mapper.Map<CompanyForReturnDto>(company));
            }
            return BadRequest("SOmething went wrong updating the company");

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id) {

            if (await _repo.DeleteCompany(id))
            {
                return Ok();
            }
            else {
                return BadRequest();
            }

        }
        //public async Task<IActionResult> SetImage(Guid companyId, [FromForm] Image);
    }
}