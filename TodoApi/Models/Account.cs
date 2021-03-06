using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        [Required]
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }
        [Required]
        public Boolean Active { get; set; }

        public String Description { get; set; }
        public String LinkedInUrl { get; set; }
        public String TwitterUrl { get; set; }
        public String InstagramUrl { get; set; }
        public Boolean VerifiedEmail { get; set; }
        
        [ForeignKey("ImageId")]
        public Image? Image { get; set; }
        public Guid? ImageId { get; set; }


        public ICollection<WorksAt> Jobs { get; set; }


        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        
        public PermissionRole Role { get; set; }

        public Guid? RoleId { get; set; }


    }
}
