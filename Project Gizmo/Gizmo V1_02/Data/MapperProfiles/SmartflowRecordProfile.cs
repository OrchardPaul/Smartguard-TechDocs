using AutoMapper;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.GadjitContext.GadjIT_App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data.MapperProfiles
{
    public class SmartflowRecordProfile : Profile
    {
        public SmartflowRecordProfile()
        {
            CreateMap<SmartflowRecords, UsrOrDefChapterManagement>();
            CreateMap<UsrOrDefChapterManagement, SmartflowRecords>()
                .ForMember(dest => dest.RowId, act => act.MapFrom(scr => scr.Id))
                .ForMember(dest => dest.Id, act => act.Ignore());

            CreateMap<VmChapter, VmChapter>();

        }
    }
}
