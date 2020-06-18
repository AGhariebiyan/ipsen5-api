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
using System.Collections.Generic;
using System.Timers;
using AutoMapper;
using GMAPI.Other;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Newtonsoft.Json.Linq;

namespace GMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly AccountsController _accounts;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, 
            IConfiguration config, 
            AccountsController accountsController,
            IAccountRepository accountRepo,
            IMapper mapper) {
            _repo = repo;
            _config = config;
            _accounts = accountsController;
            _accountRepo = accountRepo;
            _mapper = mapper;
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

            //Send verification Email
            Guid verificationId = Guid.NewGuid();
            var verification = new Verification {Id = verificationId, Account = createdAccount};
            await _repo.CreateVerificationInstance(verification);
            EmailService.SendVerificationEmail(createdAccount.Email, verificationId.ToString());
            
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
            
            //var userAccount = _accounts.GetMyAccount().Result.Value
            var userAccount = await _accountRepo.GetFullAccount(Guid.Parse(id));

            var accountToReturn = _mapper.Map<AccountForMeDto>(userAccount);
            //... manual validations return false if anything untoward is discovered
            return Ok(new {account = accountToReturn});
        }

        [HttpGet("verifications")]

        public async Task<ActionResult<IEnumerable<Verification>>> GetVerifications()
        {
            return await _repo.GetVerifications();
        }

        [HttpGet("email/verify={verificationId}")]

        public async Task<IActionResult> VerifyEmail(Guid verificationId)
        {
            var account = _repo.VerifyEmail(verificationId).Result;
            if (account == null)
            {
                return BadRequest("Link is expired");
            };
            account.VerifiedEmail = true;

            if (await _accountRepo.SaveAll()) {
                return Ok("Email has been verified");
            }
            throw new Exception("Something went wrong");


        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountForLoginDto accountForLoginDto) {
            var accountFromRepo = await _repo.Login(accountForLoginDto.Email.ToLower().Trim(), accountForLoginDto.Password);

            if (accountFromRepo == null) {
                return Unauthorized("wrong email or password");
            }

            if (!accountFromRepo.VerifiedEmail)
            {
                return Unauthorized("email not verified");
            }
           
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, accountFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Email,  accountFromRepo.Email)
            };
            if (accountFromRepo.RoleId != null) {
                claims.Add(new Claim(ClaimTypes.Role, accountFromRepo.Role.InternalName));
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