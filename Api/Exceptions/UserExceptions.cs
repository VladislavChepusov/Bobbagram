namespace Api.Exceptions
{
    public class NotFoundException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"{Model} is not found";
    }

    public class IsExistException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"Такой {Model} уже занят";
    }


    public class AuthorizationException : Exception
    {
        public override string Message => $"Вы не авторизованы!";
    }


    public class PasswordException : Exception
    {
        public override string Message => $"Неверный пароль!";
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
            Model = "User";
        }

    }
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Model = "Post";
        }

    }

    public class ContentNotFoundException : NotFoundException
    {
        public ContentNotFoundException()
        {
            Model = "Content";
        }

    }

    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException()
        {
            Model = "Comment";
        }

    }



}
