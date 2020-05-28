using GMAPI.Data;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly AccountsController _accounts;

        public AuthController(IAuthRepository repo, IConfiguration config, AccountsController accountsController) {
            _repo = repo;
            _config = config;
            _accounts = accountsController;
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

        [HttpGet("jwt/validate/{jwtToken}")]
        public async Task<IActionResult> ValidateJWT(String jwtToken)
        {
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);
            }
            catch(SecurityTokenException e)
            {
                Console.WriteLine(e.ToString());
                return Unauthorized(); 
            }
            catch(Exception e)
            { 
                System.Diagnostics.Debug.WriteLine(e.ToString()); //something else happened
                throw;
            }

            var decodedToken = tokenHandler.ReadJwtToken(jwtToken);
            var id = decodedToken.Claims.First(claim => claim.Type == "nameid").Value;
            var userAccount = _accounts.GetAccount(Guid.Parse(id)).Result.Value;
            
            //... manual validations return false if anything untoward is discovered
            return Ok(new {account = userAccount});
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