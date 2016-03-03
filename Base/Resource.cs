using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Base
{
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

	public class ResourceBase {
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			Type type = this.GetType();
			foreach (var p in type.GetProperties()) {
				var value = p.GetValue (this);
				if (value != null) {
					string valueStr = value.ToString();
					//Console.WriteLine (valueStr);
					if (value.GetType ().IsGenericType) {
						var valueEnum = value as IEnumerable;
						valueStr = valueEnum.ToStr ();
					}
					sb.AppendFormat ("{0}:\n{1}\n", p.Name, valueStr.AddTabEachLine());
				}
			}
			return sb.ToString ();	
		}

		public Dictionary<string, object> ToDict () {
			Type type = this.GetType();
			var dict = new Dictionary<string, object> ();

			foreach (var p in type.GetProperties()) {
				var value = p.GetValue (this);
				if (value != null) {
					if (value.GetType ().IsGenericType) {
						var valueEnum = value as IEnumerable;
						var valueobj = new List<object> ();
						foreach (var v in valueEnum) {
							var resource = v as ResourceBase;
							if (resource != null) {
								valueobj.Add (resource.ToDict ());
							} else {
								valueobj.Add (v);
							}
						}
						dict [p.Name] = valueobj;
					}  else {
						var resource = value as ResourceBase;
						if (resource != null) {
							dict [p.Name] = resource.ToDict ();
						} else {
							dict [p.Name] = value;
						}
					}
				}
			}
			return dict;
		}
	}

	public class User: ResourceBase
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

    public class TimeLocation: ResourceBase
    {
		public string Weeks { get; set; }
		public int Day_of_week { get; set; }
		public int Period_of_day { get; set; }
		public string Location { get; set; }
    }

    public class Course: ResourceBase
    {
        // Identifiers.
		public string Id { get; set; }
		public string Semester { get; set; }
		public string Course_number { get; set; }
		public string Course_sequence { get; set; }

        // Metadata.
		public string Name { get; set; }
		public int Credit { get; set; }
		public int Hour { get; set; }
		public string Description { get; set; }

        // Time & location.
		public List<TimeLocation> Time_locations { get; set; }

        // Staff.
		public List<User> Teachers { get; set; }
		public List<User> Assistants { get; set; }
    }

    public class Announcement
    {
        // Identifiers.
		public string id;
        public string course_id;

        // Metadata.
		public User owner;
        public string created_at;
        public int priority;
        public bool read;

        // Content.
        public string title;
        public string body;
    }

    public class File
    {
        // Identifiers.
        public string id;
        public string course_id;

        // Metadata.
		public User owner;
        public string created_at;
        public string title;
        public string description;
        public string [] category;
        public bool read;

        // Content.
        public string filename;
        public int size;
        public string download_url;
    }

    public class Attachment
    {
        public string filename;
        public int size;
        public string download_url;
    }

    public class Submission 
    {
        // Metadata.
		public User owner;
        public string created_at;
        public bool late;

        // Content.
        public string body;
		public Attachment attachment;

        // Scoring metadata.
		public User marked_by;
        public string marked_at;

        // Scoring content.
		public double mark;
        public string comment;
		public Attachment comment_attachment;
    }

    public class Homework
    {
        // Identifiers.
        public string id;
        public string course_id;

        // Metadata.
        public string created_at;
        public string begin_at;
        public string due_at;
        public int submitted_count;
        public int not_submitted_count;
        public int seen_count;
        public int marked_count;

        // Content.
        public string title;
        public string body;
		public Attachment attachment;

        // Submissions.
		public Submission [] submissions;
    }
}

