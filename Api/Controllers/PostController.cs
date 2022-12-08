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

        // Назову жэто потом "Актуально"(отсортировать по дате)

        [HttpGet]
        public async Task<List<PostModel>> GetAllPosts(int skip = 0, int take = 10)
              => await _postService.GetAllPosts(skip, take);
        [HttpGet]
        public async Task<PostModel> GetPostById(Guid id)
            => await _postService.GetPostById(id);



        // Запрос на создание поста
        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                    throw new Exception("not authorize");
    
            await _postService.CreatePost(userId,request);

        }


    }
}
