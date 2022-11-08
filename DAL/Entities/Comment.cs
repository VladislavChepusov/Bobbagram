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
        //Ид поста под котором оставлен комментарий
        public Guid PostId { get; set; }
        // Автор комментария
        public virtual User Author { get; set; } = null!;
        //Текст комментария
        public string Text { get; set; } = null!;
        // Дата|Время написания комментария 
        public DateTimeOffset MadeOn { get; set; }
        
    }
}
