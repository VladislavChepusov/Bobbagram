using Api.Models.Post;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly PostService _postService;

        public CommentController(PostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        [Authorize]
        public async Task CreateComment(CommentModel model)
        {
            var UserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.AddCommentToPost(UserId, model);
        }

        [HttpGet]
        public async Task<List<GetCommentsRequestModel>> GetComments(Guid postId)
        {
            return await _postService.GetCommentsFromPost(postId);
        }
    }
}
