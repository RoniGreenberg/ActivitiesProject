using Org.BouncyCastle.Asn1.X509;

namespace ActivitiesApi.ApiModels
{
    public class Course
    {
        public int? id;
        public string? name;
        public int? teacher_id;
        public int? category_id;
        public int? course_duration;
        public string? teacher_name;
       
    }
}
