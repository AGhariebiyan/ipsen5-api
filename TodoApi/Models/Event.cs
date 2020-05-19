using System;
using System.ComponentModel.DataAnnotations;

namespace GMAPI.Models
{
    public class Event
    {
        public Guid Id { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventLocationName { get; set; }

        [Required]
        public string EventLocationStreet { get; set; }

        [Required]
        public string EventLocationPostalCode { get; set; }

        [Required]
        public string EventLocationRegion { get; set; }

        [Required]
        public string EventLocationCountry { get; set; }

        public Account[] EventAttendees { get; set; }

        public DateTime EventDate { get; set; }
    }
}
