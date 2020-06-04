using System;

namespace GMAPI.Models
{
    public class Verification
    {
        public Guid Id { get; set; }
        public Account Account { get; set; }
        
        public Guid AccountId { get; set; }
    }
}