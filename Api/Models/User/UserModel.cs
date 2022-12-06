namespace Api.Models.User
{
    // Модель пользователя
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public int PostsCount { get; set; }
    }

    public class UserAvatarModel : UserModel
    {
        public string? AvatarLink { get; set; }
    }
}
