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
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);


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

    }
}
  