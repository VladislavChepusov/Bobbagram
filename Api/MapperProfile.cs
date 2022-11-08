using AutoMapper;
using Common;

namespace Api
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            // мапа для добавление данных
            CreateMap<Models.CreateUserModel, DAL.Entities.User>()
               .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
               .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
               //время UTC
               .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
               ;
            // мапа для отображения данных пользоватдя
            CreateMap<DAL.Entities.User, Models.UserModel>();

            CreateMap<DAL.Entities.Avatar, Models.AttachModel>();
        }
    }
}
