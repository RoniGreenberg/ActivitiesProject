using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using activitiesLibrary;
using ActivitiesApi.Services;

namespace ActivitiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public TeachersController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Teachers
        [HttpGet]
        public IActionResult GetTeachers()
        {
            var teachers = new List<Teacher>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "SELECT teacher_id, name, email, teacher_url, description FROM teachers";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    teachers.Add(new Teacher
                    {
                        teacher_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        email = reader.GetString(2),
                        teacher_url = reader.GetString(3),
                        description = reader.GetString(4)
                    });
                }

                reader.Close();
            }

            return Ok(teachers);
        }
    }
}

