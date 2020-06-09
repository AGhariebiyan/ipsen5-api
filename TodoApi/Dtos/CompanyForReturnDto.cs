using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Dtos
{
    public class CompanyForReturnDto
    {
        public Guid Id { get; set; }

        public String Name { get; set; }
        public String Address { get; set; }
        public Boolean Active { get; set; }

        public ImageDto Image { get; set; }
        
    }
}
