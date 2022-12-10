namespace Api.Models.Likes
{
    public class LikeModel
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
