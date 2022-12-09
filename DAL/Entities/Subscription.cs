using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid SubUserId { get; set; } //Тот на кого подписываются
        public User SubUser { get; set; } = null!;
        public Guid UserId { get; set; }//тот кто подписыается 
        public User User { get; set; } = null!;
    }
}
