using AutoMapper;
using GMAPI.Dtos;
using GMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GMAPI.Other
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RoleForCreateDto, PermissionRole>();
            CreateMap<Account, AccountDto>();
            CreateMap<Account, AccountForMeDto>();
            CreateMap<AccountForUpdateDto, Account>();
        }
        
    }
}
