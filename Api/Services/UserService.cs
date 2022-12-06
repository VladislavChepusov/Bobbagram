using Api.Configs;
using Api.Models.Attach;
using Api.Models.User;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;



namespace Api.Services
{
    public class UserService 
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private Func<User, string?>? _linkGenerator;
        public void SetLinkGenerator(Func<User, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        // Конструктор сервиса
        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // Сохранение данных о пользовате в БД
        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync(); 
            return t.Entity.Id;
        }

        // Вернуть список пользователей
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() =>
            (await _context.Users.AsNoTracking().Include(x => x.Avatar).ToListAsync())
                .Select(x => _mapper.Map<User, UserAvatarModel>(x, o => o.AfterMap(FixAvatar)));

        // Проверерть существует ли такой пользователь 
        public async Task<bool> CheckUserExist(string email)
        {

            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());

        }

        // Добавление аватарки пользователя
        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var avatar = new Avatar
                {
                    Author = user,
                    MimeType = meta.MimeType,
                    FilePath = filePath,
                    Name = meta.Name,
                    Size = meta.Size
                };
                user.Avatar = avatar;
                await _context.SaveChangesAsync();
            }
        }

        // Получить фотографию аватара пользователя по ID
        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var atach = _mapper.Map<AttachModel>(user.Avatar);
            return atach;
        }

        // Закрыть сессию пользователя!!
        public async Task CloseAllSessionByIdUser(Guid id_user)
        {
            // Взять метол  GetSessionById !!!!!!!!!!!!!!!
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id_user);
            if (session != null)
            {
                _context.UserSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }
  
        // Удалить пользователя из БД
        public async Task DeleteAccount(Guid id)
        {
            var dbUser = await GetUserById(id);
            if (dbUser != null)
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
        }


        // Возвращает пользователя по ID
        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null || user == default)
                throw new Exception("user not found");
            return user;
        }

        // возвращает данные текущего пользователя в контролер
        public async Task<UserAvatarModel> GetUser(Guid id) =>
                    _mapper.Map<User, UserAvatarModel>(await GetUserById(id), o => o.AfterMap(FixAvatar));


        private void FixAvatar(User s, UserAvatarModel d)
        {
            d.AvatarLink = s.Avatar == null ? null : _linkGenerator?.Invoke(s);
        }


        //Очистка данных
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
