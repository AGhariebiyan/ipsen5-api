using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
       
        public String FirstName { get; set; }
        
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        
        public String Email { get; set; }
        public Boolean Active { get; set; }
        public String LinkedInUrl { get; set; }
        public String TwitterUrl { get; set; }
        public String InstagramUrl { get; set; } 

    }
}
