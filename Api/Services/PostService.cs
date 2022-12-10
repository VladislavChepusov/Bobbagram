using Api.Configs;
using Api.Exceptions;
using Api.Models.Attach;
using Api.Models.Likes;
using Api.Models.Post;
using Api.Models.Subscriptions;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;

        public PostService(IMapper mapper, IOptions<AuthConfig> config, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task CreatePost(Guid User_id,CreatePostRequest request)
        {
            var model = _mapper.Map<CreatePostModel>(request);
            model.AuthorId = User_id;
            model.Contents.ForEach(x =>
            {
                x.AuthorId = model.AuthorId;
                x.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "attaches",
                    x.TempId.ToString());

                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    File.Move(tempFi.FullName, x.FilePath, true);
                }
            });

            var dbModel = _mapper.Map<Post>(model);
            await _context.Posts.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }


        public async Task DeletePost(Guid User_id, Guid Post_id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == Post_id);
            if (post == null)
                throw new PostNotFoundException();
            if (post.AuthorId != User_id)
                throw new Exception("You are not the author of the post");
 
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

        }

            public async Task<List<PostModel>> GetAllPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.PostContents)
                .Include(x => x.PostComments)
                .AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                .Select(x => _mapper.Map<PostModel>(x))
                .ToListAsync();

            return posts;
        }

        public async Task<List<PostModel>> GetSubPosts(int skip, int take, Guid userId)
        {
            //var user = await _context.Users.Include(x => x.Subscribes).FirstOrDefaultAsync(x => x.Id == userId);
            var subs = await _context.Subscriptions
                .Where(x => x.UserId == userId)
                .Select(x => _mapper.Map<SubscriptionModel>(x)).ToListAsync();

            List<Guid> subscribesIds = new List<Guid>();

            foreach (var sub in subs)
            {
                subscribesIds.Add(sub.SubUserId);
            }

            List<PostModel> posts = new List<PostModel>();

            posts = await _context.Posts
            .Include(x => x.Author).ThenInclude(x => x.Avatar)
            .Include(x => x.PostComments).ThenInclude(x => x.Author)
            .Include(x => x.PostContents)
            .AsNoTracking()
            .OrderByDescending(x => x.Created).Skip(skip).Take(take)
            .Where(x => subscribesIds.Contains(x.AuthorId))
            .Select(x => _mapper.Map<PostModel>(x))
            .ToListAsync();
      
            foreach (var post in posts)
            {
                post.LikesCount = (await GetPostLikes(post.Id)).Count();
            }
            return posts;
        }

        ///NTAAAAAAAKKKK
        public async Task<IEnumerable<PostLike>> GetPostLikes(Guid postId)
        {

            var likes = await _context.PostLikes.AsNoTracking()
                .Where(x => x.PostId == postId)
                .Select(x => _mapper.Map<PostLike>(x))
                .ToListAsync();
            return likes;
        }


 
        public async Task ChangePost (Guid postId,ChangePost newdata)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == newdata.Id);
            if (post == null)
                throw new PostNotFoundException();
            if (post.AuthorId != postId)
                throw new Exception("You are not the author of the post");

            post.Description = newdata.Description;

            await _context.SaveChangesAsync();
        }

        public async Task<PostModel> GetPostById(Guid id)
        {
            var post = await _context.Posts
                  .Include(x => x.Author).ThenInclude(x => x.Avatar)
                  .Include(x => x.PostContents)
                  .Include(x => x.PostComments)
                  .AsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => _mapper.Map<PostModel>(x))
                  .FirstOrDefaultAsync();
            if (post == null)
                throw new PostNotFoundException();

            return post;
        }


        public async Task<AttachModel> GetPostContent(Guid postContentId)
        {
            var res = await _context.PostContents.FirstOrDefaultAsync(x => x.Id == postContentId);
            return _mapper.Map<AttachModel>(res);
        }

    }
}
