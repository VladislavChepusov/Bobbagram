﻿using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;

namespace Api
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
            CreateMap<DAL.Entities.User, UserAvatarModel>();

            CreateMap<DAL.Entities.Avatar, AttachModel>();


            CreateMap<DAL.Entities.PostContent, AttachModel>();
            CreateMap<DAL.Entities.PostContent, AttachExternalModel>();

            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, DAL.Entities.PostContent>();


            CreateMap<CreatePostModel, DAL.Entities.Post>()
                .ForMember(d => d.PostContents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
        }
    }
}
