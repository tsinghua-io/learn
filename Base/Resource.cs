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
        public static TValue GetValueOrDefault<TValue> (IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            object val;
            try { return dictionary.TryGetValue(key, out val) ? (TValue)val : defaultValue; }
            catch { return defaultValue; }
        }

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

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            // Resolve new data, connected to document databases.
            var id = GetValueOrDefault(vals, "id", "");
            if (id == "")
            {
                // No id detected.
                return vals;
            }
            else
            {
                // Id detected, write to the database.
                var doc = database.GetDocument(id);
                doc.Update((UnsavedRevision newRevision) =>
                {
                    var properties = newRevision.Properties;
                    properties = Update(properties, vals);
                    return true;
                });

                return new Dictionary<string, object>() { { "id", id } };
            }
        }
    }

    public class SyncedUser: User
    {
        public override string Id { get; }
        public override string Name => GetAttributeOrDefault("name", ""); 
        public override string Type => GetAttributeOrDefault("type", ""); 
        public override string Department => GetAttributeOrDefault("department", ""); 
        public override string Class => GetAttributeOrDefault("class", ""); 
        public override string Gender => GetAttributeOrDefault("gender", ""); 
        public override string Email => GetAttributeOrDefault("email", ""); 
        public override string Phone => GetAttributeOrDefault("phone", ""); 

        public SyncedUser (Dictionary<string, object> vals)
        {
            // Only save the id of user.
            Id = (string)vals["id"];
        }

        private TValue GetAttributeOrDefault <TValue> (string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(Id);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
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

        public AsyncedUser () { }

        public AsyncedUser (Dictionary<string, object> vals)
        {
            // Save attributes in the instance.
            Name = GetValueOrDefault(vals, "name", "");
            Type = GetValueOrDefault(vals, "type", "");
            Department = GetValueOrDefault(vals, "department", "");
            Class = GetValueOrDefault(vals, "class", "");
            Gender = GetValueOrDefault(vals, "gender", "");
            Email = GetValueOrDefault(vals, "email", "");
            Phone = GetValueOrDefault(vals, "phone", "");
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
            Weeks = GetValueOrDefault(vals, "weeks", "");
            DayOfWeek = GetValueOrDefault(vals, "day_of_week", 0);
            PeriodOfDay = GetValueOrDefault(vals, "period_of_day", 0);
            Location = GetValueOrDefault(vals, "location", "");
        }

        public static Dictionary<string, object> ResolveNewData(Dictionary<string, object> vals) => vals;
    }

    public class Course: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("courses");

        // Identifiers.
        public string Id { get; }
        public string Semester => GetAttributeOrDefault("semester", "");
        public string CourseNumber => GetAttributeOrDefault("course_number", "");
        public string CourseSequence => GetAttributeOrDefault("course_sequence", "");

        // Metadata.
        public string Name => GetAttributeOrDefault("name", "");
        public int Credit => GetAttributeOrDefault("credit", 0);
        public int Hour => GetAttributeOrDefault("hour", 0);
        public string Description => GetAttributeOrDefault("description", "");

        // Time & location.
        public List<TimeLocation> TimeLocations
        {
            get
            {
                var time_locations = new List<TimeLocation>();
                var list = GetAttributeOrDefault("time_locations", new List<object>());
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
                var doc = database.GetExistingDocument(Id);
                var list = GetAttributeOrDefault("teachers", new List<object>());
                foreach (object teacher in list)
                {
                    var dict = (Dictionary<string, object>)teacher;
                    var id = GetValueOrDefault(dict, "id", "");
                    if (id == "") { teachers.Add(new AsyncedUser(dict)); }
                    else { teachers.Add(new SyncedUser(dict)); }
                }
                return teachers;
            }
        }

        public List<User> Assistants
        {
            get
            {
                var assistants = new List<User>();
                var doc = database.GetExistingDocument(Id);
                var list = GetAttributeOrDefault("assistants", new List<object>());
                foreach (object assistant in list)
                {
                    var dict = (Dictionary<string, object>)assistant;
                    var id = GetValueOrDefault(dict, "id", "");
                    if (id == "") { assistants.Add(new AsyncedUser(dict)); }
                    else { assistants.Add(new SyncedUser(dict)); }
                }
                return assistants;
            }
        }

        public Course (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
        }

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            // Resolve nested user objects.
            object val;
            if (vals.TryGetValue("teachers", out val))
            {
                var teachers = new List<object>();
                var list = (List<object>)val;
                list.ForEach(item => teachers.Add(User.ResolveNewData((Dictionary<string, object>)item)));
                vals["teachers"] = teachers;
            }

            if (vals.TryGetValue("assistants", out val))
            {
                var assistants = new List<object>();
                var list = (List<object>)val;
                list.ForEach(item => assistants.Add(User.ResolveNewData((Dictionary<string, object>)item)));
                vals["assistants"] = assistants;
            }

            // Save to the document.
            var id = vals["id"];
            var doc = database.GetDocument((string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }

        private TValue GetAttributeOrDefault<TValue>(string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(Id);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
        }
    }

    public class Announcement: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("announcements");

        // Identifiers.
        public string Id { get; }
        public string CourseId => GetAttributeOrDefault("course_id", "");

        // Metadata.
		public User Owner
        {
            get
            {
                var dict = GetAttributeOrDefault("owner", new Dictionary<string, object>());
                var id = GetValueOrDefault(dict, "id", "");
                if (id == "") { return new AsyncedUser(dict); }
                else { return new SyncedUser(dict); }
            }
        }
        public string CreatedAt => GetAttributeOrDefault("created_at", "");
        public int Priority => GetAttributeOrDefault("priority", 0);
        public bool Read => GetAttributeOrDefault("read", false);

        // Content.
        public string Title => GetAttributeOrDefault("title", "");
        public string Body => GetAttributeOrDefault("body", "");

        public Announcement (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
        }

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            object val;
            if (vals.TryGetValue("owner", out val))
            {
                vals["owner"] = User.ResolveNewData((Dictionary<string, object>)val);
            }

            // Save to the document.
            var id = vals["id"];
            var doc = database.GetDocument((string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }

        private TValue GetAttributeOrDefault<TValue>(string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(Id);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
        }
    }

    public class File: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("files");

        // Identifiers.
        public string Id { get; }
        public string CourseId => GetAttributeOrDefault("course_id", "");

        // Metadata.
		public User Owner
        {
            get
            {
                var dict = GetAttributeOrDefault("owner", new Dictionary<string, object>());
                var id = GetValueOrDefault(dict, "id", "");
                if (id == "") { return new AsyncedUser(dict); }
                else { return new SyncedUser(dict); }
            }
        }
        public string CreatedAt => GetAttributeOrDefault("created_at", "");
        public string Title => GetAttributeOrDefault("title", "");
        public string Description => GetAttributeOrDefault("description", "");
        public List<string> Category => GetAttributeOrDefault("category", new List<string>());
        public bool Read => GetAttributeOrDefault("read", false);

        // Content.
        public string Filename => GetAttributeOrDefault("filename", "");
        public int Size => GetAttributeOrDefault("size", 0);
        public string DownloadUrl => GetAttributeOrDefault("download_url", "");

        public File (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
        }

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            object val;
            if (vals.TryGetValue("owner", out val))
            {
                vals["owner"] = User.ResolveNewData((Dictionary<string, object>)val);
            }

            // Save to the document.
            var id = vals["id"];
            var doc = database.GetDocument((string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }

        private TValue GetAttributeOrDefault<TValue>(string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(Id);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
        }
    }

    public class Attachment: ResourceBase
    {
        public string Filename;
        public int Size;
        public string DownloadUrl;

        public Attachment (Dictionary<string, object> vals)
        {
            Filename = GetValueOrDefault(vals, "filename", "");
            Size = GetValueOrDefault(vals, "size", 0);
            DownloadUrl = GetValueOrDefault(vals, "download_url", "");
        }

        public static Dictionary<string, object> ResolveNewData(Dictionary<string, object> vals) => vals;
    }

    public class Submission: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("submissions");

        // Identifier.
        public string HomeworkId { get; }
        public string OwnerId { get; }

        // Metadata.
		public User Owner
        {
            get
            {
                var dict = GetAttributeOrDefault("owner", new Dictionary<string, object>());
                var id = GetValueOrDefault(dict, "id", "");
                if (id == "") { return new AsyncedUser(dict); }
                else { return new SyncedUser(dict); }
            }
        }
        public string CreatedAt => GetAttributeOrDefault("created_at", "");
        public bool Late => GetAttributeOrDefault("late", false);

        // Content.
        public string Body => GetAttributeOrDefault("body", "");
		public Attachment Attachment => GetAttributeOrDefault("attachment", new Attachment(new Dictionary<string, object>()));

        // Scoring metadata.
		public User MarkedBy
        {
            get
            {
                var dict = GetAttributeOrDefault("marked_by", new Dictionary<string, object>());
                var id = GetValueOrDefault(dict, "id", "");
                if (id == "") { return new AsyncedUser(dict); }
                else { return new SyncedUser(dict); }
            }
        }
        public string MarkedAt => GetAttributeOrDefault("marked_at", "");

        // Scoring content.
		public double Mark => GetAttributeOrDefault("mark", 0);
        public string Comment => GetAttributeOrDefault("comment", "");
		public Attachment CommentAttachment => GetAttributeOrDefault("comment_attachment", new Attachment(new Dictionary<string, object>()));

        public Submission (Dictionary<string, object> vals)
        {
            HomeworkId = (string)vals["homework_id"];
            OwnerId = (string)vals["owner_id"];
        }

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            object val;
            if (vals.TryGetValue("owner", out val))
            {
                vals["owner"] = User.ResolveNewData((Dictionary<string, object>)val);
            }

            if (vals.TryGetValue("marked_by", out val))
            {
                vals["marked_by"] = User.ResolveNewData((Dictionary<string, object>)val);
            }

            // Save to the document.
            var homeworkId = vals["homework_id"];
            var ownerId = vals["owner_id"];
            var doc = database.GetDocument((string)homeworkId + (string)ownerId);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }

        private TValue GetAttributeOrDefault<TValue>(string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(HomeworkId + OwnerId);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
        }
    }

    public class Homework: ResourceBase
    {
        public static Database database = Globals.manager.GetDatabase("homeworks");

        // Identifiers.
        public string Id { get; }
        public string CourseId => GetAttributeOrDefault("course_id", "");

        // Metadata.
        public string CreatedAt => GetAttributeOrDefault("created_at", "");
        public string BeginAt => GetAttributeOrDefault("begin_at", "");
        public string DueAt => GetAttributeOrDefault("due_at", "");
        public int SubmittedCount => GetAttributeOrDefault("submitted_count", 0);
        public int NotSubmittedCount => GetAttributeOrDefault("not_submitted_count", 0);
        public int SeenCount => GetAttributeOrDefault("seen_count", 0);
        public int MarkedCount => GetAttributeOrDefault("marked_count", 0);

        // Content.
        public string Title => GetAttributeOrDefault("title", "");
        public string Body => GetAttributeOrDefault("body", "");
		public Attachment Attachment => GetAttributeOrDefault("attachment", new Attachment(new Dictionary<string, object>()));

        // Submissions.
		public List<Submission> Submissions
        {
            get
            {
                var submissions = new List<Submission>();
                var list = GetAttributeOrDefault("submissions", new List<object>());
                list.ForEach(submission => submissions.Add(new Submission((Dictionary<string, object>)submission)));
                return submissions;
            }
        }

        public Homework(Dictionary<string, object> vals)
        {
            Id = GetValueOrDefault(vals, "id", "");
        }

        public static Dictionary<string, object> ResolveNewData (Dictionary<string, object> vals)
        {
            object val;
            if (vals.TryGetValue("submissions", out val))
            {
                var submissions = new List<object>();
                var list = (List<object>)val;
                list.ForEach(item => submissions.Add(Submission.ResolveNewData((Dictionary<string, object>)item)));
                vals["submissions"] = submissions;
            }

            // Save to the document.
            // Save to the document.
            var id = vals["id"];
            var doc = database.GetDocument((string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }

        private TValue GetAttributeOrDefault<TValue>(string attribute, TValue defaultValue)
        {
            var doc = database.GetExistingDocument(Id);
            var properties = doc.Properties;
            return GetValueOrDefault(properties, attribute, defaultValue);
        }
    }
}

