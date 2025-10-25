using AutoMapper;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels.Attachment;

namespace Bugget.Entities.MappingProfiles;

public class AttachmentMappingProfile : Profile
{
    public AttachmentMappingProfile()
    {
        CreateMap<AttachmentDbModel, Attachment>();
    }
}