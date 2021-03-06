using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class WorksAtForCreateDto
    {
        public Company Company { get; set; }
        public Guid CompanyId { get; set; }
        public Role Role { get; set; }
        public Guid RoleId { get; set; }
        public Boolean Accepted { get; set; }
    }
}
