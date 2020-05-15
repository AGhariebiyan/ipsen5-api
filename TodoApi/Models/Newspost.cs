using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Newspost
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool Deleted { get; set; }

        public bool Published { get; set; }

        public int AccountId { get; set; }

        public int CompanyId { get; set; }

        public bool Featured { get; set; }
    }
}
