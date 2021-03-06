using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{

    //Voorbeeldmodel!
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
    }
}
