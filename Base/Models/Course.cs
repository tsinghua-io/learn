using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Extensions;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class Schedule
    {
        public string Weeks { get; set; }

        public int Day { get; set; }

        public int Slot { get; set; }

        public string Location { get; set; }
    }

    public class BasicCourse : IResource
    {
        // Identifires.
        public string Id { get; set; }

        public string SemesterId { get; set; }

        public string Number { get; set; }

        public string Sequence { get; set; }


        // Metadata.
        public string Name { get; set; }

        public int Credit { get; set; }

        public int Hour { get; set; }

        public string Description { get; set; }

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();

        public List<BasicUser> Teachers { get; set; } = new List<BasicUser>();

        public List<BasicUser> Assistants { get; set; } = new List<BasicUser>();

        public string DocId()
        {
            return API.CourseURL(Id);
        }

        public const string RESOURCE_TYPE = "course";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public string Location()
        {
            return Schedules != null && Schedules.Count > 0 ? Schedules[0].Location : "";
        }

        public string TimeString()
        {
            if (Schedules == null)
                return "";

            var strs = new List<string>();
            foreach (var schedule in Schedules)
            {
                if (schedule.Day == 0 && schedule.Slot == 0)
                    strs.Add(schedule.Weeks);
                else
                    strs.Add(string.Format("{0}{1}-{2}", schedule.Weeks, schedule.Day, schedule.Slot));
            }
            return string.Join(" ", strs);
        }
    }

    public class Course : BasicCourse
    {
        public List<string> AnnouncementIds { get; set; } = new List<string>();

        public List<string> FileIds { get; set; } = new List<string>();

        public List<string> AssignmentIds { get; set; } = new List<string>();

        public List<Announcement> Announcements()
        {
            var list = new List<Announcement>();
            foreach (var id in AnnouncementIds)
                list.Add(Announcement.Get(Id, id));
            list.Sort((lhs, rhs) => rhs.Id.CompareTo(lhs.Id));
            return list;
        }

        public List<File> Files()
        {
            var list = new List<File>();
            foreach (var id in FileIds)
                list.Add(File.Get(Id, id));
            list.Sort((lhs, rhs) => rhs.Id.CompareTo(lhs.Id));
            return list;
        }

        public List<Assignment> Assignments()
        {
            var list = new List<Assignment>();
            foreach (var id in AssignmentIds)
                list.Add(Assignment.Get(Id, id));
            list.Sort((lhs, rhs) => lhs.DueAt.CompareTo(rhs.DueAt));
            return list;
        }

        public void SaveIds()
        {
            this.Set("AnnouncementIds", AnnouncementIds);
            this.Set("FileIds", FileIds);
            this.Set("AssignmentIds", AssignmentIds);
        }

        public bool Hide { get; set; }

        public void SaveHide()
        {
            this.Set("Hide", Hide);
        }

        public bool Ignore { get; set; }

        public void SaveIgnore()
        {
            this.Set("Ignore", Ignore);
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }


        public static Course Get(string id)
        {
            var course = new BasicCourse{ Id = id };
            return Database.Get<Course>(course.DocId());
        }

        public static Course GetExisting(string id)
        {
            var course = new BasicCourse{ Id = id };
            return Database.GetExisting<Course>(course.DocId());
        }

        public static Course FuzzyGet(string str)
        {
            // Treat as id.
            var existingCourse = GetExisting(str);
            if (existingCourse != null)
                return existingCourse;

            // Treat as name, latest first.
            var attended = Me.Get().Attended();

            foreach (var courses in attended.Values)
            {
                foreach (var course in courses)
                {
                    if (str.FuzzyMatch(course.Name))
                        return course;
                }
            }

            return null;
        }

        public bool NoMaterials()
        {
            return AnnouncementIds.Count == 0 && FileIds.Count == 0 && AssignmentIds.Count == 0;
        }
    }
}
