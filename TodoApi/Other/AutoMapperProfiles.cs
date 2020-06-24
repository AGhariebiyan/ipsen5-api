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
            CreateMap<Image, ImageDto>();
            CreateMap<Company, CompanyForReturnDto>();
            CreateMap<CompanyForUpdateDto, Company>();
            CreateMap<WorksAt, WorksAtForReturnDto>();
            CreateMap<WorksAtForCreateDto, WorksAt>();
            CreateMap<WorksAtForUpdateDto, WorksAt>();
            CreateMap<RoleForUpdateDto, Role>();
            CreateMap<AccountForPasswordDto, Account >();
        }
        
    }
}
