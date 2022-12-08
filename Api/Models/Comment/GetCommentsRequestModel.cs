namespace Api.Models.Comment
{
    public class GetCommentsRequestModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string CommentText { get; set; }
        public DateTimeOffset Created { get; set; }
        
    }
}
