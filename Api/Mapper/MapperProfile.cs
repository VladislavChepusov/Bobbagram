using Api.Mapper.MapperActions;
using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;
using DAL.Entities;

namespace Api.Mapper
{
    public class MapperProfile : Profile
    {
        // Из чего>во что мутируют данные
        public MapperProfile()
        {
            // мапа для добавление данных
            CreateMap<CreateUserModel, DAL.Entities.User>()
               .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
               //хэшируем пароль
               .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
               //время UTC
               .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));

            // мапа для отображения данных о пользователях
            CreateMap<DAL.Entities.User, UserModel>();

            CreateMap<DAL.Entities.User, UserAvatarModel>()
                //.ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDay))
                .ForMember(d => d.PostsCount, m => m.MapFrom(s => s.Posts!.Count))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<Avatar, AttachModel>();

            CreateMap<Post, PostModel>()
                .ForMember(d => d.Contents, m => m.MapFrom(d => d.PostContents))
                .ForMember(d => d.Comments, m => m.MapFrom(d => d.PostComments));



            CreateMap<CommentModel, Comment>()
                 .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                 .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<Comment, GetCommentsRequestModel>();
               // .ForMember(d => d.AuthorId, m => m.MapFrom(s => s.Author.Id));




            CreateMap<PostContent, AttachModel>();

            CreateMap<PostContent, AttachExternalModel>().AfterMap<PostContentMapperAction>();

            CreateMap<CreatePostRequest, CreatePostModel>();

            CreateMap<MetadataModel, MetadataLinkModel>();

            CreateMap<MetadataLinkModel, PostContent>();

            CreateMap<CreatePostModel, Post>()
                .ForMember(d => d.PostContents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
        }
    }
}
