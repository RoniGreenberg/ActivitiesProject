using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using activitiesLibrary;  // ודא שהמודל Course נמצא כאן
using System.Collections.Generic;
using ActivitiesApi.Services;

namespace ActivitiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CoursesController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Course
        [HttpGet]
        public IActionResult GetCourses()
        {
            var courses = new List<Course1>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                // שאילתה לשלוף את כל המידע אודות הקורסים
                string query = "SELECT course_id, name, teacher_id, course_duration, minimum_age, maximum_age, category_id, image_url FROM course";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var course = new Course1
                    {
                        course_id = reader.GetInt32(0),   // course_id
                        name = reader.GetString(1),
                        teacher_id = reader.GetInt32(2),
                        course_duration = reader.GetInt32(3),
                        minimum_age = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                        maximum_age = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                        category_id = reader.GetInt32(6),
                        image_url = reader.GetString(7)
                    };

                    // עדכון גילאים ספציפיים לקורס "טניס"
                    if (course.name == "Tennis")
                    {
                        course.minimum_age = null;  // לא להגדיר גיל מינימלי לטניס
                        course.maximum_age = null;  // לא להגדיר גיל מקסימלי לטניס
                    }

                    courses.Add(course);
                }

                reader.Close();
            }

            return Ok(courses);
        }
    }
}
