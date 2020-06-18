using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        [Required]
        public String Name { get; set; }
        public String Address { get; set; }
        public Boolean Active { get; set; }
        
        public Image? Image { get; set; }
        public Guid? ImageId { get; set; }
    }
}
