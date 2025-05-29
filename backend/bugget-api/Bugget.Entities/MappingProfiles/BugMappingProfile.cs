using AutoMapper;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.Views;

namespace Bugget.Entities.MappingProfiles;


public class BugMappingProfile : Profile
{
    public BugMappingProfile()
    {
        CreateMap<BugDbModel, Bug>();
        CreateMap<Bug, BugView>();
    }    
}