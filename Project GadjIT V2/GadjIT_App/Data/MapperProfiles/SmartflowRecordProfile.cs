using AutoMapper;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Data.MapperProfiles
{
    public class SmartflowRecordProfile : Profile
    {
        public SmartflowRecordProfile()
        {
            CreateMap<App_SmartflowRecord, Client_SmartflowRecord>()
                .ForMember(dest => dest.Id, act => act.MapFrom(scr => scr.RowId));
            
            CreateMap<Client_SmartflowRecord, App_SmartflowRecord>()
                .ForMember(dest => dest.RowId, act => act.MapFrom(scr => scr.Id))
                .ForMember(dest => dest.Id, act => act.Ignore());

            CreateMap<SmartflowV2, SmartflowV2>();

        }
    }


}
