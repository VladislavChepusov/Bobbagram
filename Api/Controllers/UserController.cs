using Api.Models.Attach;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;  
        
        public UserController(UserService userService)
        {
            _userService = userService;
            if (userService != null)
                _userService.SetLinkGenerator(x =>
                Url.Action(nameof(GetUserAvatar), new { userId = x.Id, download = false }));
        }


        // Удаление акаунта пользователя
        [HttpDelete]
        public async Task DeleteMyAccount()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            var SessionId = User.GetClaimValue<Guid>(ClaimNames.SessionId);
           
            if (userId != default)
            {
                await _userService.DeleteAccount(userId);
                await _userService.CloseAllSessionByIdUser(SessionId);
            }
            else
                throw new Exception("you are not authorized");

        }

        // Пост запрос на отправку данных и сохранение их в БД
        /*
        [HttpPost]
        public async Task CreateUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExist(model.Email))
                throw new Exception("user is exist");
            await _userService.CreateUser(model);

        }
        */


        // Гет запрос возвращение списка пользователей из БД
        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() 
            => await _userService.GetUsers();


        // Гет запрос возвращение отдельного(авторизованного) пользователя из БД
        [HttpGet]
        public async Task<UserAvatarModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);// Берем ID текущего пользователя
            if (userId != default)
            {
                return await _userService.GetUser(userId);
            }
            else
                throw new Exception("you are not authorized");

        }

        // Пост запрос добавления аватрки пользователю 
        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
                if (!tempFi.Exists)
                    throw new Exception("file not found");
                else
                {
                    // постояная папка attaches в проекте
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString()); 
                    var destFi = new FileInfo(path);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    System.IO.File.Copy(tempFi.FullName, path, true);
                    await _userService.AddAvatarToUser(userId, model, path);
                }
            }
            else
                throw new Exception("you are not authorized");
        }


        // Получить аватар пользователя по его ID
        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
        {
            var attach = await _userService.GetUserAvatar(userId);
            var fs = new FileStream(attach.FilePath, FileMode.Open);
            if (download)
                return File(fs, attach.MimeType, attach.Name);
            else
                return File(fs, attach.MimeType);
        }

        // вернуть аватар пользователю
        // если true скачать аватар пользователя по id 
        [HttpGet]
        public async Task<FileStreamResult> GetCurentUserAvatar(bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                return await GetUserAvatar(userId, download);
            }
            else
                throw new Exception("you are not authorized");

        }

    }
}
  