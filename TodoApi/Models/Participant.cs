using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Participant
    {

        [Key]
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

    }
}
