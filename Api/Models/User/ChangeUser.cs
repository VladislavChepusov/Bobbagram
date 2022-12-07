namespace Api.Models.User
{
    public class ChangeUser
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public string About { get; set; } = "";

    }
}
