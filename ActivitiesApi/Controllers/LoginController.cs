using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Data;
using ActivitiesApi.Services;

namespace ActivitiesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        // הזרקת DatabaseService
        public LoginController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            // חיפוש משתמש לפי אימייל
            string query = "SELECT UserId, FirstName, LastName, Email, Password FROM Users WHERE Email = @Email";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", loginRequest.Email);

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                return Unauthorized("User not found.");
            }

            reader.Read();

            // שליפת נתונים מהשורה
            var userId = reader.GetInt32("UserId");
            var firstName = reader.GetString("FirstName");
            var lastName = reader.GetString("LastName");
            var email = reader.GetString("Email");
            var hashedPassword = reader.GetString("Password");

            // בדיקת סיסמה
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, hashedPassword))
            {
                return Unauthorized("Incorrect password.");
            }

            // אם ההתחברות מוצלחת
            return Ok(new
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            });
        }
    }

    // מודל הבקשה
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

