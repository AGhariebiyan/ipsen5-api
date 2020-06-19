using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class WorksAtForCreateDto
    {
      
        public Guid CompanyId { get; set; }
        public Role Role { get; set; }
        public Guid RoleId { get; set; }
    }
}
