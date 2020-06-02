using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionRolesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private PostgresDatabaseContext _context;


        public PermissionRolesController(IMapper mapper, PostgresDatabaseContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleForCreateDto role) {
            var roleToCreate = _mapper.Map<PermissionRole>(role);

            _context.PermissionRoles.Add(roleToCreate);

            if (await _context.SaveChangesAsync() > 0) {
                return Ok();
            }
            return BadRequest();
        }
    }
}