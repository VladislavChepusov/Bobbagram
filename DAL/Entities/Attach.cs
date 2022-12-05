using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    // загружаемое пользователем 
    public class Attach
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!; // тип файла
        public string FilePath { get; set; } = null!; // расположения файла
        public long Size { get; set; } // размерз файла 
        public Guid AuthorId { get; set; }

        public virtual User Author { get; set; } = null!;
      
    }
}
