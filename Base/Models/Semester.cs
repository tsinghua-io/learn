using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class BasicSemester : IResource
    {
        public string Id { get; set; }

        public DateTime BeginAt { get; set; }

        public string DocId()
        {
            return API.SemesterURL();
        }

        public const string RESOURCE_TYPE = "semester";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }
    }

    public class Semester : BasicSemester
    {
        public int WeekNow()
        {
            var diff = DateTime.Now - BeginAt;
            return diff.Days / 7 + 1;
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

        public static Semester Get()
        {
            return Database.Get<Semester>(new Semester().DocId());
        }

        public static async Task Update()
        {
            Console.WriteLine("Updating semester.");
            
            var semester = await API.Semester();
            semester.Save();
            
            Console.WriteLine(string.Format("Semester updated to {0}, began at {1}.", semester.Id, semester.BeginAt));
        }
    }
}
