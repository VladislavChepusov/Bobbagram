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

    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException()
        {
            Model = "Comment";
        }

    }



}
