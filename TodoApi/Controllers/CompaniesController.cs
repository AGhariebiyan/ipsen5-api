using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using GMAPI.Data;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _repo;
        private readonly IMapper _mapper;
        public CompaniesController(ICompanyRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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

        [HttpPost]
        public async Task<IActionResult> CreateCompany(Company comp) {
            var companyToCreate = await _repo.CreateCompany(comp);
            return Ok(companyToCreate);
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