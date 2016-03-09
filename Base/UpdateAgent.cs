using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Base
{
	namespace Trivial {

		public class ResourceBase {
			static public object MakeDefault(Type type) {
				if (type.IsSubclassOf (typeof(ResourceBase))) {
					var dict = new Dictionary<string, object> ();
					foreach (var p in type.GetProperties()) {
						dict [p.Name] = MakeDefault (p.PropertyType);
					}
					return dict;
				} else if (type.IsValueType) {
					return Activator.CreateInstance(type);
				} else if (type == typeof(string)) {
					return "";
				} else {
					return Activator.CreateInstance(type);
				}
			}

			public virtual Dictionary<string, object> Save() {
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

		public delegate bool ResourceSaver(string id, Dictionary<string, object> vals);

		public class IdResourceBase: ResourceBase {
			public virtual string Id { get; set; }
			protected static Dictionary<Type, ResourceSaver> ResourceMap = new Dictionary<Type, ResourceSaver>() {
				{typeof(User), global::Base.User.ResolveNewData},
				{typeof(Course), global::Base.Course.ResolveNewData},
				{typeof(Announcement), global::Base.Announcement.ResolveNewData},
				{typeof(File), global::Base.File.ResolveNewData},
				{typeof(Submission), global::Base.Submission.ResolveNewData},
				{typeof(Homework), global::Base.Homework.ResolveNewData}
			};

			public virtual string GetId() {
				return this.Id;
			}

        	public override Dictionary<string, object> Save() {
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
									valueobj.Add (resource.Save ());
								} else {
									valueobj.Add (v);
								}
							}
							dict [p.Name] = valueobj;
						}  else {
							var resource = value as ResourceBase;
							if (resource != null) {
								// Resource type
								dict [p.Name] = resource.Save ();
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
				string id = this.GetId();
				var saver = ResourceMap [type];
				saver (id, dict);
				return new Dictionary<string, object> {
					{"Id", id}
				};
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
			public override string Id { get; set; }
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
			public override string Id { get; set; }
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
			public override string Id { get; set; }
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

			public override string GetId() {
				return String.Format("{0}/{1}", this.Homework_id, (this?.Owner?.Id) ?? "");
			}
		}

		public class Homework: IdResourceBase
		{
			// Identifiers.
			public override string Id { get; set; }
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
				// Fixme: error handling
				JsonConvert.DeserializeObject<Trivial.User> (jsonString).Save();
				return true;
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
				var courses = JsonConvert.DeserializeObject<List<Trivial.Course>> (jsonString);
				foreach (var course in courses) {
					course.Save ();
				}
				return true;
			}
		}
	} // End class UpdateAgent
}