using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    // Модель создания пользователя,с проверкой ввода обязательных полей и идентичностей паролей 
    public class CreateUserModel
    {
        //[Required] - обязательное поле
        //[Compare(nameof(Password))] - равенство полей
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string RetryPassword { get; set; }
        [Required]
        public DateTimeOffset BirthDate { get; set; }

        public string About { get; set; } 

        public CreateUserModel(string name, string email, string password, string retryPassword, DateTimeOffset birthDate, string about)
        {
            Name = name;
            Email = email;
            Password = password;
            RetryPassword = retryPassword;
            BirthDate = birthDate;
            About = about;
        }

    }
}
