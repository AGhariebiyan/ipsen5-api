using GMAPI.Data;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config) {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AccountForRegisterDto accountForRegisterDto) {

            //Remove spaces in front and back and lower the string.
            accountForRegisterDto.Email = accountForRegisterDto.Email.ToLower().Trim();
            
            //Check if the user exists
            if (await _repo.AccountExists(accountForRegisterDto.Email)) {

                return BadRequest(new { error = "Email already exists" } );
            }

            //Make new Account object based on the DTO
            var accountToCreate = new Account
            {
                Email = accountForRegisterDto.Email,
                FirstName = accountForRegisterDto.FirstName,
                LastName = accountForRegisterDto.LastName,
                MiddleName = accountForRegisterDto.MiddleName
            };

            //Create the account
            var createdAccount = await _repo.Register(accountToCreate, accountForRegisterDto.Password);


            //Return 201 (created)
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountForLoginDto accountForLoginDto) {
            var accountFromRepo = await _repo.Login(accountForLoginDto.Email.ToLower().Trim(), accountForLoginDto.Password);

            if (accountFromRepo == null) {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, accountFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Email,  accountFromRepo.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
               
        }
    }
}