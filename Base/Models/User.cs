using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class BasicUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }

    public class BasicMe: BasicUser, IResource
    {
        public string DocId()
        {
            return API.ProfileURL();
        }

        public const string RESOURCE_TYPE = "me";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public async Task Update()
        {
            Console.WriteLine("Updating profile.");

            var profile = await API.Profile();
            profile.Save();

            Console.WriteLine("Profile updated.");
        }
    }

    public class Me : BasicMe
    {
        public Me()
        {
            AttendedIds = new List<string>();
        }

        public IList<string> AttendedIds { get; set; }

        public IList<Course> Attended(string semester = null)
        {
            var list = new List<Course>();
            foreach (var id in AttendedIds)
            {
                var course = Course.Get(id);
                if (semester == null || semester == course.Semester)
                    list.Add(course);
            }
            return list;
        }

        public async Task UpdateAllAttended()
        {
            Console.WriteLine("Updating all attended courses.");

            var attended = await API.Attended("all");
            var deleted = new HashSet<string>(AttendedIds);
            
            int newCount = 0, updateCount = 0;

            AttendedIds.Clear();
            foreach (var course in attended)
            {
                AttendedIds.Add(course.Id);
                if (deleted.Remove(course.Id))
                    updateCount++;
                else
                    newCount++;
                course.Save();
            }
            this.Set("AttendedIds", AttendedIds);

            foreach (var id in deleted)
                new Course(id).Delete();
            
            Console.WriteLine(
                "All attended courses updated. ({0} new, {1} update, {2} delete)",
                newCount, updateCount, deleted.Count);
        }

        public static Me Get()
        {
            return Database.Get<Me>(new Me().DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
