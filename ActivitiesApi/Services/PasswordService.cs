using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace ActivitiesApi.Services
{
    public class PasswordService
    {
        // יצירת Hash לסיסמה באמצעות BCrypt
        public string HashPassword(string password)
        {
            // יצירת hash של הסיסמה עם BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // אימות סיסמה על ידי השוואת ההאש
        public bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            // השוואת הסיסמה שהוזנה להאש המאוחסן
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHashedPassword);
        }
    }
}
