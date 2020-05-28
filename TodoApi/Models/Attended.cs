using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Attended
    {
        public Guid Id { get; set; }
        [Required]
        public Account Account { get; set; }
        [Required]
        public FieldOfStudy Study { get; set; }
    }
}
