namespace Api.Exceptions
{
    public class NotFoundException : Exception
    {
        public string? Model { get; set; }
        public override string Message => $"{Model} не найден!";
    }

    public class IsExistException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"Такой {Model} уже занят!";
    }

    public class AuthorizationException : Exception
    {
        public override string Message => $"Вы не авторизованы!";
    }

    public class PasswordException : Exception
    {
        public override string Message => $"Неверный пароль!";
    }

    public class AgainException : Exception
    {
        public string? Model { get; set; }
        public override string Message => $"Вы уже {Model}!";
    }

    public class NoRightsException : Exception
    {
        public string? Model { get; set; }
        public override string Message => $"Вы не  {Model}!";
    }






    public class SessionNotFoundException : NotFoundException
    {
        public SessionNotFoundException()
        {
            Model = "Сессия";
        }
    }
    public class AuthorRightException : NoRightsException
    {
        public AuthorRightException()
        {
            Model = "автор контента";
        }
    }

    public class LikeNotFoundException : NotFoundException
    {
        public LikeNotFoundException()
        {
            Model = "Лайк";
        }
    }

    public class AgainLikeException : AgainException
    {
        public AgainLikeException()
        {
            Model = "лайкнули";
        }
    }
    public class AgainUnSubscribeException : AgainException
    {
        public AgainUnSubscribeException()
        {
            Model = "отписались";
        }
    }

    public class AgainSubscribeException : AgainException
    {
        public AgainSubscribeException()
        {
            Model = "подписались";
        }
    }

    public class UserNameIsExistException : IsExistException
    {
        public UserNameIsExistException()
        {
            Model = "никнейм";
        }
    }

    public class EmailIsExistException : IsExistException
    {
        public EmailIsExistException()
        {
            Model = "email";
        }
    }

    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
        {
            Model = "Пользователь";
        }
    }

    public class SubscribersNotFoundException : NotFoundException
    {
        public SubscribersNotFoundException()
        {
            Model = "Подписчик";
        }
    }

    public class SubscriptionNotFoundException : NotFoundException
    {
        public SubscriptionNotFoundException()
        {
            Model = "Подписка";
        }
    }

    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Model = "Пост";
        }
    }

    public class ContentNotFoundException : NotFoundException
    {
        public ContentNotFoundException()
        {
            Model = "Контент";
        }
    }

    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException()
        {
            Model = "Комментарий";
        }
    }



}
