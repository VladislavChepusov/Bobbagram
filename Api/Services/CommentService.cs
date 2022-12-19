using AutoMapper;
using DAL.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;
using Api.Models.Comment;
using Api.Exceptions;

namespace Api.Services
{
    public class CommentService
    {
        private readonly DAL.DataContext _context;
        private readonly IMapper _mapper;

        public CommentService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task AddCommentToPost(Guid userId, CommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            //var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            var post = await _context.Posts.Include(x => x.PostComments).FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (user == null)
                throw new UserNotFoundException();
            if (post == null)
                throw new PostNotFoundException();
            var dbcomment = _mapper.Map<Comment>(model);
            dbcomment.Author = user;
            dbcomment.AuthorId = userId;

            _context.Comments.Add(dbcomment);
            post.PostComments.Add(dbcomment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(Guid userId, Guid comId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var com = await _context.Comments.FirstOrDefaultAsync(x => x.Id == comId);
            if (user == null)
                throw new UserNotFoundException();
            if (com == null)
                throw new CommentNotFoundException();
            if (com.AuthorId != userId)
                throw new AuthorRightException();

            _context.Comments.Remove(com);
            await _context.SaveChangesAsync();
        }



        public async Task<List<GetCommentsRequestModel>> GetCommentsFromPost(Guid postId)
        {
            var post = await _context.Posts.Include(x => x.PostComments).Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == postId);
            List<GetCommentsRequestModel> comments = new List<GetCommentsRequestModel>();
            if (post != null)
            {
                foreach (var comment in post.PostComments)
                {
                    comments.Add(_mapper.Map<GetCommentsRequestModel>(comment));
                }
            }
            return comments;
        }
    }
}
