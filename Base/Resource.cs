using System.Collections.Generic;
using System.Linq;
using Couchbase.Lite;

namespace Base
{
    public static class Globals
    {
        public static Manager manager = Manager.SharedInstance;
        public static Database database = manager.GetDatabase("learn");
    }

    public class ResourceBase
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
                var doc = Globals.database.GetDocument("user/" + (string)id);
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
        public override string Name => (string)GetAttribute("name"); 
        public override string Type => (string)GetAttribute("type"); 
        public override string Department => (string)GetAttribute("department"); 
        public override string Class => (string)GetAttribute("class"); 
        public override string Gender => (string)GetAttribute("gender"); 
        public override string Email => (string)GetAttribute("email"); 
        public override string Phone => (string)GetAttribute("phone"); 

        public SyncedUser (Dictionary<string, object> vals)
        {
            // Only save the id of user.
            Id = (string)vals["id"];
        }

        private object GetAttribute (string attribute)
        {
            var doc = Globals.database.GetExistingDocument("user/" + Id);
            var val = doc.GetProperty(attribute);
            return val == null ? "" : val;
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
        // Identifiers.
        public string Id;
        public string Semester;
        public string CourseNumber;
        public string CourseSequence;

        // Metadata.
        public string Name;
        public int Credit;
        public int Hour;
        public string Description;

        // Time & location.
        public List<TimeLocation> TimeLocations;

        // Staff.
		public List<User> Teachers;
		public List<User> Assistants;

        public Course (Dictionary<string, object> vals)
        {
            Id = GetValueOrDefault(vals, "id", "");
            Semester = GetValueOrDefault(vals, "semester", "");
            CourseNumber = GetValueOrDefault(vals, "course_number", "");
            CourseSequence = GetValueOrDefault(vals, "course_sequence", "");
            Name = GetValueOrDefault(vals, "name", "");
            Credit = GetValueOrDefault(vals, "credit", 0);
            Hour = GetValueOrDefault(vals, "hour", 0);
            Description = GetValueOrDefault(vals, "description", "");

            TimeLocations = new List<TimeLocation>();
            object val;
            if (vals.TryGetValue("time_locations", out val))
            {
                var list = (List<object>)val;
                list.ForEach(item => TimeLocations.Add(new TimeLocation((Dictionary<string, object>)item)));
            }

            Teachers = new List<User>();
            if (vals.TryGetValue("teachers", out val))
            {
                var list = (List<object>)val;
                foreach (object teacher in list)
                {
                    var dict = (Dictionary<string, object>)teacher;
                    var id = GetValueOrDefault(dict, "id", "");
                    if ((string)id == "") { Teachers.Add(new AsyncedUser(dict)); }
                    else { Teachers.Add(new SyncedUser(dict)); }
                }
            }

            Assistants = new List<User>();
            if (vals.TryGetValue("assistants", out val))
            {
                var list = (List<object>)val;
                foreach (object assistant in list)
                {
                    var dict = (Dictionary<string, object>)assistant;
                    var id = GetValueOrDefault(dict, "id", "");
                    if (id == "") { Assistants.Add(new AsyncedUser(dict)); }
                    else { Assistants.Add(new SyncedUser(dict)); }
                }
            }
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
            var doc = Globals.database.GetDocument("course/" + (string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }
    }

    public class Announcement: ResourceBase
    {
        // Identifiers.
        public string Id;
        public string CourseId;

        // Metadata.
		public User Owner;
        public string CreatedAt;
        public int Priority;
        public bool Read;

        // Content.
        public string Title;
        public string Body;

        public Announcement (Dictionary<string, object> vals)
        {
            Id = GetValueOrDefault(vals, "id", "");
            CourseId = GetValueOrDefault(vals, "course_id", "");
            CreatedAt = GetValueOrDefault(vals, "created_at", "");
            Priority = GetValueOrDefault(vals, "priority", 0);
            Read = GetValueOrDefault(vals, "read", false);
            Title = GetValueOrDefault(vals, "title", "");
            Body = GetValueOrDefault(vals, "body", "");

            object val;
            if (!vals.TryGetValue("owner", out val))
            {
                // No owner.
                Owner = new AsyncedUser();
            }
            else
            {
                var dict = (Dictionary<string, object>)val;
                var id = GetValueOrDefault(dict, "id", "");
                if ((string)id == "") { Owner = new AsyncedUser(dict); }
                else { Owner = new SyncedUser(dict); }
            }
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
            var doc = Globals.database.GetDocument("announcement/" + (string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }
    }

    public class File: ResourceBase
    {
        // Identifiers.
        public string Id;
        public string CourseId;

        // Metadata.
		public User Owner;
        public string CreatedAt;
        public string Title;
        public string Description;
        public List<string> Category;
        public bool Read;

        // Content.
        public string Filename;
        public int Size;
        public string DownloadUrl;

        public File (Dictionary<string, object> vals)
        {
            Id = GetValueOrDefault(vals, "id", "");
            CourseId = GetValueOrDefault(vals, "course_id", "");
            CreatedAt = GetValueOrDefault(vals, "created_at", "");
            Title = GetValueOrDefault(vals, "title", "");
            Description = GetValueOrDefault(vals, "description", "");
            Category = (List<string>)GetValueOrDefault(vals, "category", new List<string>());
            Read = GetValueOrDefault(vals, "read", false);
            Filename = GetValueOrDefault(vals, "filename", "");
            Size = GetValueOrDefault(vals, "size", 0);
            DownloadUrl = GetValueOrDefault(vals, "download_url", "");

            object val;
            if (!vals.TryGetValue("owner", out val))
            {
                // No owner.
                Owner = new AsyncedUser();
            }
            else
            {
                var dict = (Dictionary<string, object>)val;
                var id = GetValueOrDefault(dict, "id", "");
                if ((string)id == "") { Owner = new AsyncedUser(dict); }
                else { Owner = new SyncedUser(dict); }
            }
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
            var doc = Globals.database.GetDocument("file/" + (string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
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
        // Identifier.
        public string HomeworkId;

        // Metadata.
		public User Owner;
        public string CreatedAt;
        public bool Late;

        // Content.
        public string Body;
		public Attachment Attachment;

        // Scoring metadata.
		public User MarkedBy;
        public string MarkedAt;

        // Scoring content.
		public double Mark;
        public string Comment;
		public Attachment CommentAttachment;

        public Submission (Dictionary<string, object> vals)
        {
            HomeworkId = GetValueOrDefault(vals, "homework_id", "");
            CreatedAt = GetValueOrDefault(vals, "created_at", "");
            Late = GetValueOrDefault(vals, "late", false);
            Body = GetValueOrDefault(vals, "body", "");
            MarkedAt = GetValueOrDefault(vals, "marked_at", "");
            Mark = GetValueOrDefault(vals, "mark", 0);
            Comment = GetValueOrDefault(vals, "comment", "");
            Attachment = new Attachment(GetValueOrDefault(vals, "attachment", new Dictionary<string, object>()));
            CommentAttachment = new Attachment(GetValueOrDefault(vals, "comment_attachment", new Dictionary<string, object>()));

            object val;
            if (!vals.TryGetValue("owner", out val))
            {
                Owner = new AsyncedUser();
            }
            else
            {
                var dict = (Dictionary<string, object>)val;
                var id = GetValueOrDefault(dict, "id", "");
                if ((string)id == "") { Owner = new AsyncedUser(dict); }
                else { Owner = new SyncedUser(dict); }
            }

            if (!vals.TryGetValue("marked_by", out val))
            {
                MarkedBy = new AsyncedUser();
            }
            else
            {
                var dict = (Dictionary<string, object>)val;
                var id = GetValueOrDefault(dict, "id", "");
                if ((string)id == "") { MarkedBy = new AsyncedUser(dict); }
                else { MarkedBy = new SyncedUser(dict); }
            }
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
            var owner = (Dictionary<string, object>)vals["owner"];
            var ownerId = owner["id"];
            var doc = Globals.database.GetDocument("file/" + (string)homeworkId + (string)ownerId);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }
    }

    public class Homework: ResourceBase
    {
        // Identifiers.
        public string Id;
        public string CourseId;

        // Metadata.
        public string CreatedAt;
        public string BeginAt;
        public string DueAt;
        public int SubmittedCount;
        public int NotSubmittedCount;
        public int SeenCount;
        public int MarkedCount;

        // Content.
        public string Title;
        public string Body;
		public Attachment Attachment;

        // Submissions.
		public List<Submission> Submissions;

        public Homework(Dictionary<string, object> vals)
        {
            Id = GetValueOrDefault(vals, "id", "");
            CourseId = GetValueOrDefault(vals, "course_id", "");
            CreatedAt = GetValueOrDefault(vals, "created_at", "");
            BeginAt = GetValueOrDefault(vals, "begin_at", "");
            DueAt = GetValueOrDefault(vals, "due_at", "");
            SubmittedCount = GetValueOrDefault(vals, "submitted_count", 0);
            NotSubmittedCount = GetValueOrDefault(vals, "not_submitted_count", 0);
            SeenCount = GetValueOrDefault(vals, "seen_count", 0);
            MarkedCount = GetValueOrDefault(vals, "marked_count", 0);
            Title = GetValueOrDefault(vals, "title", "");
            Body = GetValueOrDefault(vals, "body", "");
            Attachment = new Attachment(GetValueOrDefault(vals, "attachment", new Dictionary<string, object>()));

            Submissions = new List<Submission>();
            object val;
            if (vals.TryGetValue("submissions", out val))
            {
                var list = (List<object>)val;
                list.ForEach(submission => Submissions.Add(new Submission((Dictionary<string, object>)submission)));
            }
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
            var doc = Globals.database.GetDocument("homework/" + (string)id);
            doc.Update((UnsavedRevision newRevision) =>
            {
                var properties = newRevision.Properties;
                properties = Update(properties, vals);
                return true;
            });

            return vals;
        }
    }
}

