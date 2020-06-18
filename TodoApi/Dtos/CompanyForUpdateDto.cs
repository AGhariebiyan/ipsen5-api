using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class CompanyForUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public String Name { get; set; }
    }
}
