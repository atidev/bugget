using AutoMapper;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.Views;

namespace Bugget.Entities.MappingProfiles;

public sealed class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<CommentDbModel, Comment>();
        CreateMap<Comment, CommentView>();
    }
}