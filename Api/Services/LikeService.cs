using Api.Models.Likes;
using AutoMapper;
using DAL.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;
using Api.Exceptions;
using System.Collections.Generic;

namespace Api.Services
{
    public class LikeService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public LikeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task AddLikePost(Guid userId, LikeRequest newLike)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var post = await _context.Posts
                .Include(it => it.Likes)
                .FirstOrDefaultAsync(x => x.Id == newLike.EntityId);
            if (post == null)
                throw new PostNotFoundException();
            
            var isExist = post.Likes!.FirstOrDefault(it => it.UserId == userId);
            if (isExist != null)
                throw new Exception("Like already exists");
            
            var dblike = _mapper.Map<PostLike>(newLike);
            dblike.User = user;
            dblike.UserId = userId;
            dblike.Post = post;

            await _context.PostLikes.AddAsync(dblike);
            await _context.SaveChangesAsync();
        }


        public async Task AddLikeComment(Guid userId, LikeRequest newLike)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var comment = await _context.Comments
                .Include(it => it.Likes)
                .FirstOrDefaultAsync(x => x.Id == newLike.EntityId);
            if (comment == null)
                throw new CommentNotFoundException();

            var isExist = comment.Likes!.FirstOrDefault(it => it.UserId == userId);
            if (isExist != null)
                throw new Exception("Like already exists");

            var dblike = _mapper.Map<CommentLike>(newLike);
            dblike.User = user;
            dblike.UserId = userId;
            dblike.Comment = comment;

            await _context.CommentLikes.AddAsync(dblike);
            await _context.SaveChangesAsync();
        }


       
        public async Task DeleteLikePost(Guid userId, LikeRequest newLike)
        {
            var post = await _context.Posts
               .Include(it => it.Likes)
               .FirstOrDefaultAsync(x => x.Id == newLike.EntityId);
            if (post == null)
                throw new PostNotFoundException();

            var like = post.Likes!.FirstOrDefault(it => it.UserId == userId);

            if (like != null)
            {
                _context.PostLikes.Remove(like);
                await _context.SaveChangesAsync();
               
            }
            else
            throw new Exception("You didn't like this post");
        }


        public async Task DeleteLikeComment(Guid userId, LikeRequest newLike)
        {
            var comment = await _context.Comments
               .Include(it => it.Likes)
               .FirstOrDefaultAsync(x => x.Id == newLike.EntityId);
            if (comment == null)
                throw new CommentNotFoundException();

            var like = comment.Likes!.FirstOrDefault(it => it.UserId == userId);

            if (like != null)
            {
                _context.CommentLikes.Remove(like);
                await _context.SaveChangesAsync();

            }
            else
                throw new Exception("You didn't like this comment");
        }



      
        public async Task<IEnumerable<PostLike>> GetPostLikes(Guid postId)
        {
           
            var likes = await _context.PostLikes.AsNoTracking()
                .Where(x => x.PostId == postId)
                .Select(x => _mapper.Map<PostLike>(x))
                .ToListAsync();
            return likes;
        }



        public async Task<IEnumerable<CommentLike>> GetCommentLikes(Guid commentId)
        {
            var likes = await _context.CommentLikes.AsNoTracking()
                .Where(x => x.CommentId == commentId)
                .Select(x => _mapper.Map<CommentLike>(x))
                .ToListAsync();
 
            return likes;
        }

    }
}
