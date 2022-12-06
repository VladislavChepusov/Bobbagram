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
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService, LinkGeneratorService links)
        {
            _userService = userService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpGet]
        public async Task<UserAvatarModel> GetUserById(Guid userId)
        {

            return await _userService.GetUser(userId);
        }


            // Удаление акаунта пользователя (ВЫНЕСТИ ЛОГИКУ В СЕРВИС!!!!!!!)
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

    }
}
  