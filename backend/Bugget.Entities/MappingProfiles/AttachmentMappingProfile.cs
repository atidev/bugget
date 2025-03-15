using AutoMapper;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels;
using Bugget.Entities.Views;

namespace Bugget.Entities.MappingProfiles;

public class AttachmentMappingProfile : Profile
{
    public AttachmentMappingProfile()
    {
        CreateMap<AttachmentDbModel, Attachment>();
        CreateMap<Attachment, AttachmentView>();
    }
}