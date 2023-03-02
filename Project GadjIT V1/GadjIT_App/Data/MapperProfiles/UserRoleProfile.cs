using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_AppContext.GadjIT_App;
using Microsoft.AspNetCore.Identity;

namespace GadjIT_App.Data.MapperProfiles
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<List<ApplicationRole>, List<AspNetRoles>>();
            CreateMap<List<AspNetRoles> , List<ApplicationRole>>();

            CreateMap<ApplicationRole, AspNetRoles>();
            CreateMap<AspNetRoles, ApplicationRole>();

            CreateMap<ApplicationUser, AspNetUser>();
            CreateMap<AspNetUser, ApplicationUser>();

        }

    }
}
