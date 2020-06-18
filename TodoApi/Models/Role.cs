using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        
        public Boolean CanEditCompany { get; set; }

    }
}
