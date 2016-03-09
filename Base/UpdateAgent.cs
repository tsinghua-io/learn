using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Base
{
	namespace Trivial {

		public class ResourceBase {
			static public object MakeDefault(Type type) {
				if (type.IsSubclassOf (ResourceBase)) {
					var dict = new Dictionary<string, object> ();
					foreach (var p in type.getProperties()) {
						dict [p.Name] = MakeDefault (p.PropertyType);
					}

					return dict;
				} else if (type.IsValueType) {
					return default(type);
				} else if (type == typeof(string)) {
					return "";
				} else {
					return Activator.CreateInstance(type);
				}
			}

			public virtual Dictionary<string, object> save() {
				Type type = this.GetType();
				var dict = new Dictionary<string, object> ();

				foreach (var p in type.GetProperties()) {
					var value = p.GetValue (this);
					if (value != null) {
						dict [p.Name] = value;
					} else {
						// Fill in default values.
						dict [p.Name] = MakeDefault (p.PropertyType);
					}
				}
				return dict;
			}
		}// End class ResourceBase

		public class IdResourceBase: ResourceBase {
			protected static Dictionary<Type, Type> ResourceMap = new Dictionary() {
				{typeof(User), typeof(global::Base.User)},
				{typeof(Course), typeof(global::Base.Course)},
				{typeof(Announcement), typeof(global::Base.Announcement)},
				{typeof(File), typeof(global::Base.File)},
				{typeof(Submission), typeof(global::Base.Submission)},
				{typeof(Homework), typeof(global::Base.Homework)}
			};

        	public override Dictionary<string, object> save() {
				Type type = this.GetType();
				var dict = new Dictionary<string, object> ();

				foreach (var p in type.GetProperties()) {
					var value = p.GetValue (this);
					if (value != null) {
						if (value.GetType ().IsGenericType) {
							// Generics: List<T>
							var valueEnum = value as IEnumerable;
							var valueobj = new List<object> ();
							foreach (var v in valueEnum) {
								var resource = v as ResourceBase;
								if (resource != null) {
									valueobj.Add (resource.save ());
								} else {
									valueobj.Add (v);
								}
							}
							dict [p.Name] = valueobj;
						}  else {
							var resource = value as ResourceBase;
							if (resource != null) {
								// Resource type
								dict [p.Name] = resource.save ();
							} else {
								dict [p.Name] = value;
							}
						}
					} else {
						// Fill in default values
						dict [p.Name] = MakeDefault(p.PropertyType);
					}		
				}
				// Call the saver
				var saver = 
				return dict;
			}
		} // End class IdResourceBase

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

		public class Course: IdResourceBase
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

		public class Announcement: IdResourceBase
		{
			// Identifiers.
			public string Id { get; set; }
			public string Course_id { get; set; }

			// Metadata.
			public User Owner { get; set; }
			public string Created_at { get; set; }
			public int Priority { get; set; }
			public bool Read { get; set; }

			// Content.
			public string Title { get; set; }
			public string Body { get; set; }
		}

		public class File: IdResourceBase
		{
			// Identifiers.
			public string Id { get; set; }
			public string Course_id { get; set; }

			// Metadata.
			public User Owner { get; set; }
			public string Created_at { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
			public List<string> Category { get; set; }
			public bool Read { get; set; }

			// Content.
			public string Filename { get; set; }
			public int Size { get; set; }
			public string Download_url { get; set; }
		}

		public class Attachment: ResourceBase
		{
			public string Filename { get; set; }
			public int Size { get; set; }
			public string Download_url { get; set; }
		}

		public class Submission: IdResourceBase
		{
			// Metadata.
			public User Owner { get; set; }
			public string Homework_id { get; set; }
			public string Created_at { get; set; }
			public bool Late { get; set; }

			// Content.
			public string Body { get; set; }
			public Attachment Attachment { get; set; }

			// Scoring metadata.
			public User Marked_by { get; set; }
			public string Marked_at { get; set; }

			// Scoring content.
			public double Mark { get; set; }
			public string Comment { get; set; }
			public Attachment Comment_attachment { get; set; }
		}

		public class Homework: IdResourceBase
		{
			// Identifiers.
			public string Id { get; set; }
			public string Course_id { get; set; }

			// Metadata.
			public string Created_at { get; set; }
			public string Begin_at { get; set; }
			public string Due_at { get; set; }
			public int Submitted_count { get; set; }
			public int Not_submitted_count { get; set; }
			public int Seen_count { get; set; }
			public int Marked_count { get; set; }

			// Content.
			public string Title { get; set; }
			public string Body { get; set; }
			public Attachment Attachment { get; set; }

			// Submissions.
			public List<Submission>  Submissions;
		}
	} // End namespace Trivial

	public class UpdateAgent
	{
		private APIWrapper apiWrapper;

		public UpdateAgent (string baseUrl, string userName, string password)
		{
			apiWrapper = new APIWrapper (baseUrl, userName, password);
		}

		public bool UpdateProfile ()
		{
			string jsonString;
			var status = apiWrapper.GetProfile (out jsonString);
			if (!status.IsScuccessStatusCode ()) {
				// Get profile information fail
				return false;
			} else {
				return JsonConvert.DeserializeObject<User> (jsonString);
			}
		}

		public bool UpdateAttended (string semester)
		{
			string jsonString;
			var status = apiWrapper.GetAttended (semester, out jsonString);
			if (!status.IsScuccessStatusCode ()) {
				// Get attended information fail
				return false;
			} else {
				return JsonConvert.DeserializeObject<List<Course>> (jsonString);
			}
		}
	} // End class UpdateAgent
}