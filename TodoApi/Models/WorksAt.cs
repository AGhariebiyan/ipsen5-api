using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class WorksAt
    {
        public Guid Id { get; set; }
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public Company Company { get; set; }
        public Guid CompanyId { get; set; }
        [Required]
        public Role Role { get; set; }
        public Guid RoleId { get; set; }
        public Boolean Accepted { get; set; }

    }
}
