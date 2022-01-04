using AutoMapper;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.AppContext.GadjIT_App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Data.MapperProfiles
{
    public class SmartflowRecordProfile : Profile
    {
        public SmartflowRecordProfile()
        {
            CreateMap<SmartflowRecords, UsrOrsfSmartflows>()
                .ForMember(dest => dest.Id, act => act.MapFrom(scr => scr.RowId));
            
            CreateMap<UsrOrsfSmartflows, SmartflowRecords>()
                .ForMember(dest => dest.RowId, act => act.MapFrom(scr => scr.Id))
                .ForMember(dest => dest.Id, act => act.Ignore());

            CreateMap<VmChapter, VmChapter>();

        }
    }


}
