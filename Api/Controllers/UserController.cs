using Api.Models;
using Api.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;  
        
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Пост запрос на отправку данных и сохранение их в БД
        [HttpPost]
        // public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);
        public async Task CreateUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExist(model.Email))
                throw new Exception("user is exist");
            await _userService.CreateUser(model);

        }

        // Гет запрос возвращение списка пользователей из БД
        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GeteUsers() => await _userService.GeteUsers();

        // Гет запрос возвращение отдельного(авторизованного) пользователя из БД
        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value; // Берем ID текущего пользователя
            if (Guid.TryParse(userIdString, out var userId)) // Если можно спарсить строку в userID то
            {

                return await _userService.GetUser(userId);
            }
            else
                throw new Exception("you are not authorized");

        }

        // Пост запрос добавления аватрки пользователю 
        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
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



        
        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attach = await _userService.GetUserAvatar(userId);

            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        
        [HttpGet]
        public async Task<FileResult> DownloadAvatar(Guid userId)
        {
            var attach = await _userService.GetUserAvatar(userId);

            HttpContext.Response.ContentType = attach.MimeType;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType)
            {
                FileDownloadName = attach.Name
            };

            return result;
        }

    }
}
  