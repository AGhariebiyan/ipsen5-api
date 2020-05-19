using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class AccountForRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        //TODO Determine password requirements
        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        
        public string MiddleName { get; set; }
             
    }
}
