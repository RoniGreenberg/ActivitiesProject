using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using activitiesLibrary;
using ActivitiesApi.Services;

namespace ActivitiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly PasswordService _passwordService;

        public UserController(DatabaseService databaseService, PasswordService passwordService)
        {
            _databaseService = databaseService;
            _passwordService = passwordService;
        }

        // POST: api/User/SignUp
        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] signUpRequest user)
        {
            if (string.IsNullOrEmpty(user.FirstName) ||
                string.IsNullOrEmpty(user.LastName) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                using (MySqlConnection connection = _databaseService.GetConnection())
                {
                    connection.Open();

                    // בדיקת אימייל קיים
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", user.Email);
                        var count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            return Conflict("Email already exists.");
                        }
                    }

                    // הצפנת סיסמה
                    string hashedPassword = _passwordService.HashPassword(user.Password);

                    // הוספת משתמש חדש
                    string insertQuery = @"INSERT INTO users (first_name, last_name, email, password, age, role) 
                                           VALUES (@FirstName, @LastName, @Email, @Password, @Age, @Role)";
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        insertCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        insertCommand.Parameters.AddWithValue("@Email", user.Email);
                        insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCommand.Parameters.AddWithValue("@Age", user.Age);
                        insertCommand.Parameters.AddWithValue("@Role", "User");

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("User registered successfully.");
                        }
                        else
                        {
                            return StatusCode(500, "Failed to register user.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/User/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            try
            {
                using (MySqlConnection connection = _databaseService.GetConnection())
                {
                    connection.Open();

                    // קבלת פרטי המשתמש
                    string query = "SELECT password FROM users WHERE email = @Email";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", loginRequest.Email);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader.GetString(0);

                                // אימות סיסמה
                                if (_passwordService.VerifyPassword(loginRequest.Password, storedHashedPassword))
                                {
                                    return Ok("Login successful.");
                                }
                                else
                                {
                                    return Unauthorized("Invalid email or password.");
                                }
                            }
                            else
                            {
                                return Unauthorized("Invalid email or password.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
