﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    // Класс для ORM (пользователь)
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "empty";
        public string Email { get; set; } = "empty";
        public string PasswordHash { get; set; } = "empty";
        public DateTimeOffset BirthDate { get; set; }
        public string About { get; set; } = "empty";

        public virtual Avatar? Avatar { get; set; }


        public virtual ICollection<UserSession>? Sessions { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<Subscription>? Subscribers { get; set; }

        public virtual ICollection<CommentLike>? CommentLikes { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }

    }
}
