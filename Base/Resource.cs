using System.Collections.Generic;
using System.Linq;
using Couchbase.Lite;

namespace Base
{
    public static class Globals
    {
        public static Manager manager = Manager.SharedInstance;
    }

    public abstract class ResourceBase
    {
        public static IDictionary<string, object> Update (IDictionary<string, object> original, IDictionary<string, object> newDict)
        {
            newDict.ToList().ForEach(item => original[item.Key] = item.Value);
            return original;
        }
    }

	public abstract class User: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("users");

        public abstract string Id { get; }
		public abstract string Name { get; }
		public abstract string Type { get; }
		public abstract string Department { get; }
		public abstract string Class { get; }
		public abstract string Gender { get; }
		public abstract string Email { get; }
		public abstract string Phone { get; }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // Resolve new data, connected to document databases.
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }

    public class SyncedUser: User
    {
        private Document doc;

        public override string Id { get; }
        public override string Name => (string)doc.GetProperty("name"); 
        public override string Type => (string)doc.GetProperty("type"); 
        public override string Department => (string)doc.GetProperty("department"); 
        public override string Class => (string)doc.GetProperty("class"); 
        public override string Gender => (string)doc.GetProperty("gender"); 
        public override string Email => (string)doc.GetProperty("email"); 
        public override string Phone => (string)doc.GetProperty("phone"); 

        public SyncedUser (string id)
        {
            Id = id;
            doc = database.GetDocument(Id);
        }
    }

    public class AsyncedUser: User
    {
        public override string Id => "";
        public override string Name { get; }
        public override string Type { get; }
        public override string Department { get; }
        public override string Class { get; }
        public override string Gender { get; }
        public override string Email { get; }
        public override string Phone { get; }

        public AsyncedUser (Dictionary<string, object> vals)
        {
            // Save attributes in the instance.
            Name = (string)vals["name"];
            Type = (string)vals["type"];
            Department = (string)vals["department"];
            Class = (string)vals["class"];
            Gender = (string)vals["gender"];
            Email = (string)vals["email"];
            Phone = (string)vals["phone"];
        }
    }

    public class TimeLocation: ResourceBase
    {
        public string Weeks;
        public int DayOfWeek;
        public int PeriodOfDay;
        public string Location;

        public TimeLocation (Dictionary<string, object> vals)
        {
            Weeks = (string)vals["weeks"];
            DayOfWeek = (int)vals["day_of_week"];
            PeriodOfDay = (int)vals["period_of_day"];
            Location = (string)vals["location"];
        }
        // public static Dictionary<string, object> ResolveNewData(Dictionary<string, object> vals) => vals;
    }

    public class Course: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("courses");
        public Document doc;

        // Identifiers.
        public string Id { get; }
        public string Semester => (string)doc.GetProperty("semester");
        public string CourseNumber => (string)doc.GetProperty("course_number");
        public string CourseSequence => (string)doc.GetProperty("course_sequence");

        // Metadata.
        public string Name => (string)doc.GetProperty("name");
        public int Credit => (int)doc.GetProperty("credit");
        public int Hour => (int)doc.GetProperty("hour");
        public string Description => (string)doc.GetProperty("description");

        // Time & location.
        public List<TimeLocation> TimeLocations
        {
            get
            {
                var time_locations = new List<TimeLocation>();
                var list = (List<object>)doc.GetProperty("time_locations");
                list.ForEach(item => time_locations.Add(new TimeLocation((Dictionary<string, object>)item)));
                return time_locations;
            }
        }

        // Staff.
		public List<User> Teachers
        {
            get
            {
                var teachers = new List<User>();
                var list = (List<object>)doc.GetProperty("teachers");
                list.ForEach(item => teachers.Add(new AsyncedUser((Dictionary<string, object>)item)));
                return teachers;
            }
        }

        public List<User> Assistants
        {
            get
            {
                var assistants = new List<User>();
                var list = (List<object>)doc.GetProperty("assistants");
                list.ForEach(item => assistants.Add(new AsyncedUser((Dictionary<string, object>)item)));
                return assistants;
            }
        }

        public Course (string id)
        {
            Id = id;
            doc = database.GetDocument(Id);
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // Save to the document.
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }

    public class Announcement: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("announcements");
        private Document doc;

        // Identifiers.
        public string Id { get; }
        public string CourseId => (string)doc.GetProperty("course_id");

        // Metadata.
		public User Owner => new AsyncedUser((Dictionary<string, object>)doc.GetProperty("owner"));
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public int Priority => (int)doc.GetProperty("priority");
        public bool Read => (bool)doc.GetProperty("read");

        // Content.
        public string Title => (string)doc.GetProperty("title");
        public string Body => (string)doc.GetProperty("body");

        public Announcement (string id)
        {
            Id = id;
            doc = database.GetDocument(Id);
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // Save to the document.
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }

    public class File: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("files");
        private Document doc;

        // Identifiers.
        public string Id { get; }
        public string CourseId => (string)doc.GetProperty("course_id");

        // Metadata.
        public User Owner => new AsyncedUser((Dictionary<string, object>)doc.GetProperty("owner"));
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public string Title => (string)doc.GetProperty("title");
        public string Description => (string)doc.GetProperty("description");
        public List<string> Category => (List<string>)doc.GetProperty("category");
        public bool Read => (bool)doc.GetProperty("read");

        // Content.
        public string Filename => (string)doc.GetProperty("filename");
        public int Size => (int)doc.GetProperty("size");
        public string DownloadUrl => (string)doc.GetProperty("download_url");

        public File (string id)
        {
            Id = id;
            doc = database.GetDocument(Id);
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // Save to the document.
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }

    public class Attachment: ResourceBase
    {
        public string Filename;
        public int Size;
        public string DownloadUrl;

        public Attachment (Dictionary<string, object> vals)
        {
            Filename = (string)vals["filename"];
            Size = (int)vals["size"];
            DownloadUrl = (string)vals["download_url"];
        }

        // public static Dictionary<string, object> ResolveNewData(Dictionary<string, object> vals) => vals;
    }

    public class Submission: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("submissions");
        private Document doc;

        // Identifier.
        public string HomeworkId { get; }
        public string OwnerId { get; }

        // Metadata.
		public User Owner => new AsyncedUser((Dictionary<string, object>)doc.GetProperty("owner"));
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public bool Late => (bool)doc.GetProperty("late");

        // Content.
        public string Body => (string)doc.GetProperty("body");
		public Attachment Attachment => new Attachment((Dictionary<string, object>)doc.GetProperty("attachment"));

        // Scoring metadata.
		public User MarkedBy => new AsyncedUser((Dictionary<string, object>)doc.GetProperty("marked_by"));
        public string MarkedAt => (string)doc.GetProperty("marked_at");

        // Scoring content.
		public double Mark => (double)doc.GetProperty("mark");
        public string Comment => (string)doc.GetProperty("comment");
		public Attachment CommentAttachment => new Attachment((Dictionary<string, object>)doc.GetProperty("comment_attachment"));

        public Submission (string homeworkId, string ownerId)
        {
            HomeworkId = homeworkId;
            OwnerId = ownerId;
            doc = database.GetDocument(HomeworkId + ":" + OwnerId);
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // id is <homework_id>:<owner_id>
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }

    public class Homework: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("homeworks");
        private Document doc;

        // Identifiers.
        public string Id { get; }
        public string CourseId => (string)doc.GetProperty("course_id");

        // Metadata.
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public string BeginAt => (string)doc.GetProperty("begin_at");
        public string DueAt => (string)doc.GetProperty("due_at");
        public int SubmittedCount => (int)doc.GetProperty("submitted_count");
        public int NotSubmittedCount => (int)doc.GetProperty("not_submitted_count");
        public int SeenCount => (int)doc.GetProperty("seen_count");
        public int MarkedCount => (int)doc.GetProperty("marked_count");

        // Content.
        public string Title => (string)doc.GetProperty("title");
        public string Body => (string)doc.GetProperty("body");
		public Attachment Attachment => new Attachment((Dictionary<string, object>)doc.GetProperty("attachment"));

        // Submissions.
		public List<Submission> Submissions
        {
            get
            {
                var submissions = new List<Submission>();
                var list = (List<object>)doc.GetProperty("submissions");
                list.ForEach(submission => submissions.Add(new Submission((Dictionary<string, object>)submission)));
                return submissions;
            }
        }

        public Homework(string id)
        {
            Id = id;
            doc = database.GetDocument(Id);
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // Save to the document.
            var doc = database.GetDocument(id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return true;
        }
    }
}

