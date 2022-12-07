using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class ChangeUserPassword
    {

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string RetryPassword { get; set; }

        public ChangeUserPassword(string oldPassword, string newPassword, string retryPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
            RetryPassword = retryPassword;
        }
    }
}
