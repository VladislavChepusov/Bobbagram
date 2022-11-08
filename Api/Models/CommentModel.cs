namespace Api.Models
{
    public class CommentModel
    {
        public Guid PostId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset MadeOn { get; set; }
    
    }
}
