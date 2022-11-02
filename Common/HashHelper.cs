using System.Security.Cryptography;
using System.Text;

namespace Common
{
    // Класс помошник для реализации хэширования паролей 
    public static class HashHelper
    {
        // Вернуть зашифрованную строку
        public static string GetHash(string input)
      {
          using (var sha = SHA256.Create())
          {
              var data = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
              var sb = new StringBuilder();

              foreach (var c in data)
                  sb.Append(c.ToString("x2"));
              return sb.ToString();

          }
      }
      
        // Проверяем впускать ли пользователя в систему( сравниваем два хэш от введеного пароля с хранящимся )
        public static bool Verify(string input, string hash)
        {
            var hashInput = GetHash(input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashInput, hash) == 0;
        }

    }
}