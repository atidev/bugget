using AutoMapper;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Attachment;

namespace Bugget.Entities.MappingProfiles;

public class AttachmentMappingProfile : Profile
{
    public AttachmentMappingProfile()
    {
        CreateMap<AttachmentDbModel, Attachment>();
        CreateMap<Attachment, AttachmentObsoleteView>();
    }
}