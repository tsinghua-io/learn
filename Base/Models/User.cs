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
    }

    public class Me : BasicMe
    {
        public Dictionary<string, List<string>> AttendedIds { get; set; } = new Dictionary<string, List<string>>();

        public SortedDictionary<string, List<Course>> Attended(string semesterId = null)
        {
            var attended = new SortedDictionary<string, List<Course>>(
                               Comparer<string>.Create((lhs, rhs) => rhs.CompareTo(lhs)));
            foreach (var pair in AttendedIds)
            {
                if (semesterId != null && semesterId != pair.Key)
                    continue;
                
                var list = new List<Course>();
                foreach (var id in pair.Value)
                    list.Add(Course.Get(id));
                
                list.Sort((lhs, rhs) => lhs.Name.CompareTo(rhs.Name));
                attended[pair.Key] = list;
            }
            return attended;
        }

        public void SaveAttendedIds()
        {
            this.Set("AttendedIds", AttendedIds);
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }


        public static async Task UpdateAttended()
        {
            Console.WriteLine("Updating attended courses.");

            var attended = await API.Attended("all");
            var me = Get();
            me.AttendedIds.Clear();

            foreach (var course in attended)
            {
                if (!me.AttendedIds.ContainsKey(course.SemesterId))
                    me.AttendedIds[course.SemesterId] = new List<string>();
                me.AttendedIds[course.SemesterId].Add(course.Id);
                course.Save();
            }
            me.SaveAttendedIds();

            Console.WriteLine("Attended courses updated, {0} fetched.", attended.Count);
        }

        public static async Task UpdateMaterials(string semesterId = null)
        {
            var me = Get();
            if (me.AttendedIds.Count == 0)
                return;

            var ids = new List<string>();
            foreach (var pair in me.AttendedIds)
            {
                if (semesterId == null || pair.Key == semesterId)
                    ids.AddRange(pair.Value);
            }
            Console.WriteLine("Updating course materials for {0}.", string.Join(",", ids));

            var materials = await API.CoursesMaterials(ids);

            for (int i = 0; i < ids.Count; i++)
            {
                var course = Course.Get(ids[i]);

                course.AnnouncementIds.Clear();
                course.FileIds.Clear();
                course.AssignmentIds.Clear();
                foreach (var announcement in materials[i].Announcements)
                {
                    announcement.Save();
                    course.AnnouncementIds.Add(announcement.Id);
                }
                foreach (var file in materials[i].Files)
                {
                    file.Save();
                    course.FileIds.Add(file.Id);
                }
                foreach (var assignment in materials[i].Assignments)
                {
                    assignment.Save();
                    course.AssignmentIds.Add(assignment.Id);
                }

                course.SaveIds();
            }

            Console.WriteLine("Course materials for {0} updated.", string.Join(", ", ids));
        }

        public static Me Get()
        {
            return Database.Get<Me>(new BasicMe().DocId());
        }

        public static async Task Update()
        {
            Console.WriteLine("Updating profile.");

            var profile = await API.Profile();
            profile.Save();

            Console.WriteLine("Profile updated.");
        }
    }
}
