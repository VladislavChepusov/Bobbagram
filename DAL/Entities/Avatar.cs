using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    // аватарка пользователя
    public class Avatar : Attach
    {
        public Guid OwnerId { get; set; }


        public virtual User Owner { get; set; } = null!;
       
    }
}
