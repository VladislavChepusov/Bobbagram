using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public string CommentText { get; set; } = null!;

        public ICollection<CommentLike>? Likes { get; set; }
    }
}

