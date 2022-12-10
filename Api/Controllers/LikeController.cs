using Api.Models.Likes;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class LikeController : ControllerBase
    {
        private readonly LikeService _likeService;

        public LikeController(LikeService likeService)
        {
            _likeService = likeService;
        }


 


        [HttpPost]
        public async Task LikeThePost(LikeRequest likeRequest)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("not authorize");
            await _likeService.AddLikePost(userId,likeRequest);
        }

        [HttpPost]
        public async Task LikeTheComment(LikeRequest likeRequest)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("not authorize");
            await _likeService.AddLikeComment(userId, likeRequest);
        }

        [HttpDelete]
        public async Task DeleteLikeFromPost(LikeRequest likeRequest)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("not authorize");
            await _likeService.DeleteLikePost(userId, likeRequest);
        }

        [HttpDelete]
        public async Task DeleteLikeFromComment(LikeRequest likeRequest)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("not authorize");
            await _likeService.DeleteLikeComment(userId, likeRequest);
        }



        [HttpGet]
        public async Task<IEnumerable<PostLike>> GetPostLikes(Guid postId)
        {
            return await _likeService.GetPostLikes(postId);
        }

        
        [HttpGet]
        public async Task<IEnumerable<CommentLike>> GetCommentLikes(Guid commentId)
        {
            return await _likeService.GetCommentLikes(commentId);
        }
        
    }
}
