using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using activitiesLibrary;
using ActivitiesApi.Services;
using System.Security.Cryptography;
using System.Text;

namespace ActivitiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public SignUpController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost]
        public IActionResult SignUp([FromBody] UserSignUpRequest signUpRequest)
        {
            if (string.IsNullOrEmpty(signUpRequest.FirstName) ||
                string.IsNullOrEmpty(signUpRequest.LastName) ||
                string.IsNullOrEmpty(signUpRequest.Email) ||
                string.IsNullOrEmpty(signUpRequest.Password))
            {
                return BadRequest("All fields are required.");
            }

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if email exists
                        string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                        using (MySqlCommand checkCommand = new MySqlCommand(checkEmailQuery, connection, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@Email", signUpRequest.Email);

                            int emailCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                            if (emailCount > 0)
                            {
                                return Conflict(new { message = "Email already exists.", field = "email" });
                            }
                        }

                        // Insert new user
                        string insertQuery = "INSERT INTO users (first_name, last_name, email, password, age, Role) " +
                                             "VALUES (@FirstName, @LastName, @Email, @Password, @Age, @Role)";
                        using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@FirstName", signUpRequest.FirstName);
                            insertCommand.Parameters.AddWithValue("@LastName", signUpRequest.LastName);
                            insertCommand.Parameters.AddWithValue("@Email", signUpRequest.Email);
                            insertCommand.Parameters.AddWithValue("@Password", HashPassword(signUpRequest.Password));
                            insertCommand.Parameters.AddWithValue("@Age", signUpRequest.Age ?? (object)DBNull.Value);
                            insertCommand.Parameters.AddWithValue("@Role", UserRole.User);

                            int rowsAffected = insertCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return Ok(new { message = "User registered successfully." });
                            }
                            else
                            {
                                transaction.Rollback();
                                return StatusCode(500, "Failed to register user.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"An error occurred: {ex.Message}");
                    }
                }
            }
        }

        private string HashPassword(string password) //מצפין
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        //צריך ליצור הצפנה של סיסמה
        //varifyPassword - מחזיר את הסיסמה מההצפנה לרגיל
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            using var sha256 = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(inputPassword);
            var inputHash = sha256.ComputeHash(inputBytes);
            var inputHashString = Convert.ToBase64String(inputHash);

            // Compare the input hash with the stored hash
            return inputHashString == storedHashedPassword;
        }

    }

    public class UserSignUpRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Age { get; set; }
    }

    public enum UserRole
    {
        User = 0,
        Admin = 1
    }
}