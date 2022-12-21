using Api.Exceptions;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        public PostController(PostService postService, LinkGeneratorService links)
        {
            _postService = postService;

            links.LinkContentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId = x.Id,
            });
            links.LinkAvatarGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        // Назову жэто потом "Актуальное/Интересное"

        [HttpGet]
        public async Task<List<PostModel>> GetAllPosts(int skip = 0, int take = 10)
              => await _postService.GetAllPosts(skip, take);


        [HttpGet]
        public async Task<List<PostModel>> GetSubscriptionPosts(int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new AuthorizationException();
            //throw new Exception("not authorize");

            return  await _postService.GetSubPosts(skip, take, userId);
        }

        [HttpPut]
        public async Task ChangePosts(ChangePost newData)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new AuthorizationException();
            //throw new Exception("not authorize");

             await _postService.ChangePost(userId, newData);
        }


        [HttpGet]
        public async Task<List<PostModel>> GetPostByUserId(Guid id)
           => await _postService.GetPostByUserId(id);


        [HttpGet]
        public async Task<List<PostModel>> GetPostByUserName(String UserName)
          => await _postService.GetPostByUserName(UserName);


        [HttpGet]
        public async Task<PostModel> GetPostById(Guid id)
            => await _postService.GetPostById(id);


        // Запрос на создание поста
        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new AuthorizationException();
            //throw new Exception("not authorize");
    
            await _postService.CreatePost(userId,request);

        }


        // Запрос на создание поста
        [HttpDelete]
        public async Task DeletePost(Guid PostId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new AuthorizationException();
            //throw new Exception("not authorize");

            await _postService.DeletePost(userId, PostId);
        }
    }
}
