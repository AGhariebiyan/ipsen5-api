using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class RoleForUpdateDto
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
    }
}
