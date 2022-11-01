using Api.Models;
using Api.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
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
        public async Task<List<UserModel>> GeteUsers() => await _userService.GeteUsers();
         
    }
}
  