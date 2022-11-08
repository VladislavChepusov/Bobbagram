using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostContent:Attach
    {
        public virtual UserPost Post { get; set; } = null!;
    }
}
