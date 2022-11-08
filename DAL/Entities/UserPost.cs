using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class UserPost
    {
        public Guid Id { get; set; }
        public virtual User Author { get; set; } = null!;

      

        //public virtual ICollection



        // Описание поста
        public string? Description { get; set; }

        // Дата|время создания поста
        //Делать ли дату изменения поста????
        public DateTimeOffset MadeOn { get; set; }




        // Лист из контента пользователя в посте
        // public List<Attach> UserContent { get; set; } = new List<Attach>();
        //путь файлов
        // public string[] AttachPaths { get; set; }
        //Список комментариев от сторонних пользователей
        // public List<Comment> Comments { get; set; } = new List<Comment>();

    }
}
