using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class WorksAtForReturnDto
    {
        public Guid Id { get; set; }
     
        public AccountDto Account { get; set; }
        
        public CompanyForReturnDto Company { get; set; }
        
        public Role Role { get; set; }

    }
}
