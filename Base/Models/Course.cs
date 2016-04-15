using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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

        public string Semester { get; set; }

        public string Number { get; set; }

        public string Sequence { get; set; }


        // Metadata.
        public string Name { get; set; }

        public int Credit { get; set; }

        public int Hour { get; set; }

        public string Description { get; set; }

        public List<Schedule> Schedules { get; set; }

        public List<BasicUser> Teachers { get; set; }

        public List<BasicUser> Assistants { get; set; }

        public string DocId()
        {
            return API.CourseURL(Id);
        }

        public const string RESOURCE_TYPE = "course";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }
    }

    public class Course : BasicCourse
    {
        public Course()
        {
            AnnouncementIds = new List<string>();
            FileIds = new List<string>();
            AssignmentIds = new List<string>();
        }

        public Course(string id)
            : this()
        {
            Id = id;
        }

        public IList<string> AnnouncementIds { get; set; }

        public IList<string> FileIds { get; set; }

        public IList<string> AssignmentIds { get; set; }

        public IList<Announcement> Announcements()
        {
            var list = new List<Announcement>();
            foreach (var id in AnnouncementIds)
                list.Add(Announcement.Get(Id, id));
            return list;
        }

        public IList<File> Files()
        {
            var list = new List<File>();
            foreach (var id in FileIds)
                list.Add(File.Get(Id, id));
            return list;
        }

        public IList<Assignment> Assignments()
        {
            var list = new List<Assignment>();
            foreach (var id in AssignmentIds)
                list.Add(Assignment.Get(Id, id));
            return list;
        }

        public async Task UpdateAnnouncements()
        {
            Console.WriteLine("Updating announcements for course {0}.", Id);

            var announcements = await API.CourseAnnouncements(Id);
            var deleted = new HashSet<string>(AnnouncementIds);

            int newCount = 0, updateCount = 0;

            AnnouncementIds.Clear();
            foreach (var announcement in announcements)
            {
                AnnouncementIds.Add(announcement.Id);
                if (deleted.Remove(announcement.Id))
                    updateCount++;
                else
                    newCount++;
                announcement.Save();
            }
            this.Set("AnnouncementIds", AnnouncementIds);

            foreach (var id in deleted)
                new Announcement(Id, id).Delete();

            Console.WriteLine(
                "Announcements for course {0} updated. ({1} new, {2} update, {3} delete)",
                Id, newCount, updateCount, deleted.Count);
        }

        public async Task UpdateFiles()
        {
            Console.WriteLine("Updating files for course {0}.", Id);

            var files = await API.CourseFiles(Id);
            var deleted = new HashSet<string>(FileIds);
            
            int newCount = 0, updateCount = 0;

            FileIds.Clear();
            foreach (var file in files)
            {
                FileIds.Add(file.Id);
                if (deleted.Remove(file.Id))
                    updateCount++;
                else
                    newCount++;
                file.Save();
            }
            this.Set("FileIds", FileIds);

            foreach (var id in deleted)
                new File(Id, id).Delete();
            
            Console.WriteLine(
                "Files for course {0} updated. ({1} new, {2} update, {3} delete)",
                Id, newCount, updateCount, deleted.Count);
        }

        public async Task UpdateAssignments()
        {
            Console.WriteLine("Updating assignments for course {0}.", Id);

            var assignments = await API.CourseAssignments(Id);
            var deleted = new HashSet<string>(AssignmentIds);
            
            int newCount = 0, updateCount = 0;

            AssignmentIds.Clear();
            foreach (var assignment in assignments)
            {
                AssignmentIds.Add(assignment.Id);
                if (deleted.Remove(assignment.Id))
                    updateCount++;
                else
                    newCount++;
                assignment.Save();
            }
            this.Set("AssignmentIds", AssignmentIds);

            foreach (var id in deleted)
                new Assignment(Id, id).Delete();

            Console.WriteLine(
                "Assignments for course {0} updated. ({1} new, {2} update, {3} delete)",
                Id, newCount, updateCount, deleted.Count);
        }

        public async Task UpdateStuff()
        {
            await Task.WhenAll(UpdateAnnouncements(), UpdateFiles(), UpdateAssignments());
        }

        public bool Hide { get; set; }

        public bool Ignore { get; set; }

        public static Course Get(string id)
        {
            return Database.GetExisting<Course>(new Course(id).DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
