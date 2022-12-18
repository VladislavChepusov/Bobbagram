using Api.Configs;
using Api.Exceptions;
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


        // Изменение пароля пользователя
        public async Task ChangePassword(Guid userId, ChangeUserPassword model)
        {
           //var user = GetUserById(userId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            if (!HashHelper.Verify(model.OldPassword, user.PasswordHash))
                throw new PasswordException();



            user.PasswordHash = HashHelper.GetHash(model.NewPassword);
            await CloseAllSessionByIdUser(userId); //Закрыть все сесси пользователя (Оставить??)
            await _context.SaveChangesAsync();

           
        }


        // Изменение данных о пользовате в БД(УБРАТЬ ХАРДКОД)
        public async Task ChangeUser(Guid userId, ChangeUser model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
             if (user == null)
                throw new UserNotFoundException();

            if (user.About != model.About)
                user.About = model.About;

            if (user.Email != model.Email)
                user.Email = model.Email;

            if (user.Name != model.Name)
                user.Name = model.Name;

            if  (user.BirthDate != model.BirthDate)
                 user.BirthDate = model.BirthDate;
            await _context.SaveChangesAsync();
           
        }


        // Вернуть список пользователей
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() =>
                    await _context.Users.AsNoTracking()
                    .Include(x => x.Avatar)
                    .Include(x => x.Posts)
                    .Select(x => _mapper.Map<UserAvatarModel>(x))
                    .ToListAsync();


        // Проверерть существует ли такой пользователь  по емайлу
        public async Task<bool> CheckUserExist(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        // Проверерть существует ли такой пользователь по имени 
        public async Task<bool> CheckUserNameExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Name.ToLower() == username.ToLower());
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

        // Закрыть сессии пользователя
        public async Task CloseAllSessionByIdUser(Guid id_user)
        {
            var session = await _context.UserSessions.Where(x => x.UserId == id_user).ToListAsync();  
            if (session != null)
            {   
                _context.UserSessions.RemoveRange(session);    
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
            var user = await _context.Users.Include(x => x.Avatar).Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null || user == default)
                throw new UserNotFoundException();
            return user;
        }


        // возвращает данные текущего пользователя в контролер
        public async Task<UserAvatarModel> GetUser(Guid id) =>
            _mapper.Map<User, UserAvatarModel>(await GetUserById(id));


        //Очистка данных
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
