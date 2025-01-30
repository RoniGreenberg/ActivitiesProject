using activitiesLibrary;

namespace ActivitiesApi.Services
{
    public class UserService
    {
        // משתנים לשמירת פרטי המשתמש המחובר
        public int? UserId { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? Email { get; private set; }
        public string? CardNumber { get; private set; }
        public string? SecretCode { get; private set; }
        public string? DateValidity { get; private set; }
        public int? Age { get; private set; }
        public UserRole Role { get; private set; }

        // התחברות המשתמש ושמירת הנתונים
        public void LoginUser(User user)
        {
            UserId = user.user_id;
            FirstName = user.first_name;
            LastName = user.last_name;
            Email = user.email;
            //CardNumber = user.card_number; // ניתן להפעיל אם צריך
            //SecretCode = user.secret_code; // ניתן להפעיל אם צריך
            //DateValidity = user.date_validity; // ניתן להפעיל אם צריך
            Age = user.age;
            Role = user.Role;
        }

        // איפוס הנתונים
        public void ClearUser()
        {
            UserId = null;
            FirstName = null;
            LastName = null;
            Email = null;
            //CardNumber = null;
            //SecretCode = null;
            //DateValidity = null;
            Age = null;
            Role = 0;
        }

        // פונקציה לקבלת שם המשתמש (למשל עבור הצגת ה-Welcome)
        public string GetUserFullName()
        {
            return $"{FirstName} {LastName}";
        }

        // הפונקציה הזו כבר קיימת, אז ניתן להחליף אותה ב-GetUserFullName
        public string GetUserName()
        {
            return $"{FirstName} {LastName}";
        }
        // הוספת מאפיין לשם המשתמש המלא
        public string UserFullName { get; private set; }

        // פונקציה להחזרת המשתמש המחובר
        public User? GetLoggedInUser()
        {
            return new User
            {
                user_id = UserId,
                first_name = FirstName,
                last_name = LastName,
                email = Email,
                age = Age,
                Role = Role
            };
        }
    }
}
