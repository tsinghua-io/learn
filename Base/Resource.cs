using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Couchbase.Lite;

namespace Base
{
    public static class Globals
    {
        public static Manager manager = Manager.SharedInstance;
        public static Database database = manager.GetDatabase("learn");
    }

	public abstract class User
    {        
		public abstract string Id { get; }
		public abstract string Name { get; }
		public abstract string Type { get; }
		public abstract string Department { get; }
		public abstract string @Class { get; }
		public abstract string Gender { get; }
		public abstract string Email { get; }
		public abstract string Phone { get; }

        public static User ResolveNewData (Dictionary<string, object> vals)
        {
            // Resolve new data, connected to document databases.
            object id;
            vals.TryGetValue("id", out id);
            if (id == null || (string)id == "")
            {
                // No id detected.
                return new AsyncedUser(vals);
            }
            else
            {
                // Id detected, write to the database.
                var doc = Globals.database.GetDocument("users");
                doc.Update((UnsavedRevision newRevision) =>
                {
                    var properties = newRevision.Properties;
                    properties[(string)id] = vals;
                    return true;
                });

                return new SyncedUser(vals);
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
            var doc = Globals.database.GetExistingDocument("users");
            var user = doc.GetProperty(Id);
            return ((Dictionary<string, object>)user)[attribute];
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
            var defaultValue = new Dictionary<string, object>() {
                {"name", ""},
                {"type", ""},
                {"department", ""},
                {"class", ""},
                {"gender", ""},
                {"email", ""},
                {"phone", ""}
            };
            defaultValue.ToList().ForEach(pair => defaultValue[pair.Key] = vals[pair.Key]);

            Name = (string)defaultValue["name"];
            Type = (string)defaultValue["type"];
            Department = (string)defaultValue["department"];
            Class = (string)defaultValue["class"];
            Gender = (string)defaultValue["gender"];
            Email = (string)defaultValue["email"];
            Phone = (string)defaultValue["phone"];
        }
    }

    public class TimeLocation
    {
        public string Weeks;
        public int DayOfWeek;
        public int PeriodOfDay;
        public string Location;

        public TimeLocation (Dictionary<string, object> vals)
        {
            var defaultValue = new Dictionary<string, object>() {
                {"weeks", ""},
                {"day_of_week", 0},
                {"period_of_day", 0},
                {"location", ""}
            };
            defaultValue.ToList().ForEach(pair => defaultValue[pair.Key] = vals[pair.Key]);
            
            Weeks = (string)defaultValue["weeks"];
            DayOfWeek = (int)defaultValue["day_of_week"];
            PeriodOfDay = (int)defaultValue["period_of_day"];
            Location = (string)defaultValue["location"];
        }

        public static TimeLocation ResolveNewData(Dictionary<string, object> vals) => new TimeLocation(vals);
    }

    public class Course
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
        public TimeLocation [] TimeLocations;

        // Staff.
		public User [] Teachers;
		public User [] Assistants;
    }

    public class Announcement
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
    }

    public class File
    {
        // Identifiers.
        public string Id;
        public string CourseId;

        // Metadata.
		public User Owner;
        public string CreatedAt;
        public string Title;
        public string Description;
        public string [] Category;
        public bool Read;

        // Content.
        public string Filename;
        public int Size;
        public string DownloadUrl;
    }

    public class Attachment
    {
        public string Filename;
        public int Size;
        public string DownloadUrl;
    }

    public class Submission 
    {
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
    }

    public class Homework
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
		public Submission [] Submissions;
    }
}

