using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Couchbase.Lite;

using Newtonsoft.Json.Linq;


namespace Base
{
    public static class Globals
    {
        public static Manager manager = Manager.SharedInstance;
    }

	public static class ResourceExtensionMethods
	{

		public static string ToStr (this IEnumerable resources)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var resource in resources) {
				sb.AppendLine ("-----------------");
				sb.Append (resource.ToString ());
			}
			sb.AppendLine ("-----------------");
			return sb.ToString ();
		}

		public static string AddTabEachLine (this string str)
		{
			return str.Trim().Split ('\n').Select (s => "\t" + s).Aggregate ((i, j) => i + "\n" + j) + "\n";
		}
	}

    public abstract class ResourceBase
    {
        public static IDictionary<string, object> Update (IDictionary<string, object> original, IDictionary<string, object> newDict)
        {
            newDict.ToList().ForEach(item => original[item.Key] = item.Value);
            return original;
        }

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			Type type = this.GetType();
			foreach (var p in type.GetProperties()) {
				var value = p.GetValue (this);
				if (value != null) {
					string valueStr = value.ToString();
					if (value.GetType ().IsGenericType) {
						var valueEnum = value as IEnumerable;
						valueStr = valueEnum.ToStr ();
					}
					sb.AppendFormat ("{0}:\n{1}\n", p.Name, valueStr.AddTabEachLine());
				}
			}
			return sb.ToString ();	
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

        public SyncedUser (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
            doc = database.GetExistingDocument(Id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
        }
    }

    public class AsyncedUser: User
    {
        public override string Id { get; }
        public override string Name { get; }
        public override string Type { get; }
        public override string Department { get; }
        public override string Class { get; }
        public override string Gender { get; }
        public override string Email { get; }
        public override string Phone { get; }

		public AsyncedUser (JObject obj) : this(obj.ToObject<Dictionary<string, object>>()) {}

        public AsyncedUser (Dictionary<string, object> vals)
        {
            // Save attributes in the instance.
            Id = (string)vals["id"];
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
		public string Weeks { get; set; }
		public int DayOfWeek { get; set; }
		public int PeriodOfDay { get; set; }
		public string Location { get; set; }

		public TimeLocation (JObject obj) : this(obj.ToObject<Dictionary<string, object>>()) {}

        public TimeLocation (Dictionary<string, object> vals)
        {
            Weeks = (string)vals["weeks"];
			DayOfWeek = Convert.ToInt32(vals["day_of_week"]);
			PeriodOfDay = Convert.ToInt32(vals["period_of_day"]);
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
		public int Credit => Convert.ToInt32(doc.GetProperty("credit"));
			
		public int Hour => Convert.ToInt32(doc.GetProperty("hour"));
        public string Description => (string)doc.GetProperty("description");

        // Time & location.
        public List<TimeLocation> TimeLocations
        {
            get
            {
				var timeLocations = new List<TimeLocation>();
				var objList = (IEnumerable<object>)doc.GetProperty("time_locations");

				foreach (var obj in objList) {
					timeLocations.Add (new TimeLocation ((JObject)obj));
				}
				return timeLocations;
            }
        }

        // Staff.
		public List<User> Teachers
        {
            get
            {
				var teachers = new List<User>();
				var objList = (IEnumerable<object>)doc.GetProperty("teachers");
				
				foreach (var obj in objList) {
					teachers.Add (new AsyncedUser ((JObject)obj));
				}
				return teachers;
            }
        }

        public List<User> Assistants
        {
            get
            {
                var assistants = new List<User>();
				var objList = (IEnumerable<object>)doc.GetProperty("assistants");

				foreach (var obj in objList) {
					assistants.Add (new AsyncedUser ((JObject)obj));
				}
                return assistants;
            }
        }

        public Course (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
            doc = database.GetExistingDocument(Id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
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
        public static Database database
        {
            get
            {
                var db = Globals.manager.GetDatabase("announcements");
                var view = db.GetView("announcementsByCourseId");
                view.SetMap((doc, emit) =>
                {
                    var courseId = (string)doc["course_id"];
                    var id = (string)doc["id"];
                    emit(courseId, id);
                }, "1");
                return db;
            }
        }
        private Document doc;

        // Identifiers.
        public string Id { get; }
        public string CourseId => (string)doc.GetProperty("course_id");

        // Metadata.
		public User Owner => new AsyncedUser((JObject)doc.GetProperty("owner"));
        public string CreatedAt => (string)doc.GetProperty("created_at");
		public int Priority => Convert.ToInt32(doc.GetProperty("priority"));
        public bool Read => (bool)doc.GetProperty("read");

        // Content.
        public string Title => (string)doc.GetProperty("title");
        public string Body => (string)doc.GetProperty("body");

        public Announcement (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
            doc = database.GetExistingDocument(Id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
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
        public static Database database
        {
            get
            {
                var db = Globals.manager.GetDatabase("files");
                var view = db.GetView("filesByCourseId");
                view.SetMap((doc, emit) =>
                {
                    var courseId = (string)doc["course_id"];
                    var id = (string)doc["id"];
                    emit(courseId, id);
                }, "1");
                return db;
            }
        }
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
		public int Size => Convert.ToInt32(doc.GetProperty("size"));
        public string DownloadUrl => (string)doc.GetProperty("download_url");

        public File (Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
            doc = database.GetExistingDocument(Id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
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
		public string Filename { get; set; }
		public int Size { get; set; }
		public string DownloadUrl { get; set; }

		public Attachment (JObject obj) : this(obj.ToObject<Dictionary<string, object>>()) {}

        public Attachment (Dictionary<string, object> vals)
        {
			Filename = Convert.ToString(vals["filename"]);
			Size = Convert.ToInt32(vals["size"]);
			DownloadUrl = Convert.ToString(vals["download_url"]);
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
		public User Owner => new AsyncedUser((JObject)doc.GetProperty("owner"));
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public bool Late => (bool)doc.GetProperty("late");

        // Content.
        public string Body => (string)doc.GetProperty("body");
		public Attachment Attachment => new Attachment((JObject)doc.GetProperty("attachment"));

        // Scoring metadata.
		public User MarkedBy => new AsyncedUser((JObject)doc.GetProperty("marked_by"));
        public string MarkedAt => (string)doc.GetProperty("marked_at");

        // Scoring content.
		public double Mark => (double)doc.GetProperty("mark");
        public string Comment => (string)doc.GetProperty("comment");
		public Attachment CommentAttachment => new Attachment((JObject)doc.GetProperty("comment_attachment"));

		public Submission (JObject obj) : this(obj.ToObject<Dictionary<string, object>>()) {}

        public Submission (Dictionary<string, object> vals)
        {
			string id = (string)vals["id"];
			char[] seperator = {'/'};
			string[] words = id.Split(seperator, 2);

			HomeworkId = words[0];
			OwnerId = words[1];

            doc = database.GetExistingDocument(id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
        }

        public static bool ResolveNewData (string id, Dictionary<string, object> vals)
        {
            // id is <homework_id>/<owner_id>
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
        public static Database database
        {
            get
            {
                var db = Globals.manager.GetDatabase("homeworks");
                var view = db.GetView("homeworksByCourseId");
                view.SetMap((doc, emit) =>
                {
                    var courseId = (string)doc["course_id"];
                    var id = (string)doc["id"];
                    emit(courseId, id);
                }, "1");
                return db;
            }
        }
        private Document doc;

        // Identifiers.
        public string Id { get; }
        public string CourseId => (string)doc.GetProperty("course_id");

        // Metadata.
        public string CreatedAt => (string)doc.GetProperty("created_at");
        public string BeginAt => (string)doc.GetProperty("begin_at");
        public string DueAt => (string)doc.GetProperty("due_at");
		public int SubmittedCount => Convert.ToInt32(doc.GetProperty("submitted_count"));
		public int NotSubmittedCount => Convert.ToInt32(doc.GetProperty("not_submitted_count"));
		public int SeenCount => Convert.ToInt32(doc.GetProperty("seen_count"));
		public int MarkedCount => Convert.ToInt32(doc.GetProperty("marked_count"));

        // Content.
        public string Title => (string)doc.GetProperty("title");
        public string Body => (string)doc.GetProperty("body");
		public Attachment Attachment => new Attachment((JObject)doc.GetProperty("attachment"));

        // Submissions.
		public List<Submission> Submissions
        {
            get
			{
				var submissions = new List<Submission> ();
				var objList =  (IEnumerable<object>)doc.GetProperty("submissions");

				foreach (var obj in objList) {
					submissions.Add (new Submission ((JObject)obj));
				}
                return submissions;
            }
        }

        public Homework(Dictionary<string, object> vals)
        {
            Id = (string)vals["id"];
            doc = database.GetExistingDocument(Id);
            if (doc == null) { throw new LearnBaseException("Id not exist!"); }
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

