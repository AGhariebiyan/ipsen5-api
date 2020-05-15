using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public String Location { get; set; }
        public String Url { get; set; }
    }
}
