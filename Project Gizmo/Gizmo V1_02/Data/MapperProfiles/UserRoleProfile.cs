using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.Gizmo_Authentification;
using Microsoft.AspNetCore.Identity;

namespace Gizmo_V1_02.Data.MapperProfiles
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<List<IdentityRole>, List<AspNetRoles>>();
            CreateMap<List<AspNetRoles> , List<IdentityRole>>();

            CreateMap<IdentityRole, AspNetRoles>();
            CreateMap<AspNetRoles, IdentityRole>();

        }

    }
}
