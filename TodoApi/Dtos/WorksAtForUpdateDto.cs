using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class WorksAtForUpdateDto
    {
        public Guid Id { get; set; }
        public RoleForUpdateDto Role { get; set; }
    }
}
