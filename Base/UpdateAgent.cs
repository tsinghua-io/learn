using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Newtonsoft.Json;

namespace Base
{
	namespace Trivial
	{

		public class ResourceBase
		{
			static public object MakeDefault (Type type)
			{
				// Console.WriteLine ("make default {0}", type.ToString ());
				if (type.IsSubclassOf (typeof(ResourceBase))) {
					var dict = new Dictionary<string, object> ();
					foreach (var p in type.GetProperties()) {
						var name = p.Name.ToLower ();
						dict [name] = MakeDefault (p.PropertyType);
					}
					return dict;
				} else if (type.IsValueType) {
					return Activator.CreateInstance (type);
				} else if (type == typeof(string)) {
					return "";
				} else {
					return Activator.CreateInstance (type);
				}
			}

			public virtual Dictionary<string, object> Save ()
			{
				Type type = this.GetType ();
				var dict = new Dictionary<string, object> ();

				foreach (var p in type.GetProperties()) {
					var name = p.Name.ToLower ();
					var value = p.GetValue (this);
					if (value != null) {
						dict [name] = value;
					} else {
						// Fill in default values.
						dict [name] = MakeDefault (p.PropertyType);
					}
				}
				return dict;
			}
		}
// End class ResourceBase

		public delegate bool ResourceSaver (string id, Dictionary<string, object> vals);

		public class IdResourceBase: ResourceBase
		{
			public virtual string Id { get; set; }

			protected static Dictionary<Type, ResourceSaver> ResourceMap = new Dictionary<Type, ResourceSaver> () {
				{ typeof(Course), global::Base.Course.ResolveNewData },
				{ typeof(Announcement), global::Base.Announcement.ResolveNewData },
				{ typeof(File), global::Base.File.ResolveNewData },
				{ typeof(Submission), global::Base.Submission.ResolveNewData },
				{ typeof(Homework), global::Base.Homework.ResolveNewData }
			};

			public virtual string GetId ()
			{
				return this.Id;
			}

			public override Dictionary<string, object> Save ()
			{
				Type type = this.GetType ();
				var dict = new Dictionary<string, object> ();

				foreach (var p in type.GetProperties()) {
					var name = p.Name.ToLower ();
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
							dict [name] = valueobj;
						} else {
							var resource = value as ResourceBase;
							if (resource != null) {
								// Resource type
								dict [name] = resource.Save ();
							} else {
								dict [name] = value;
							}
						}
					} else {
						// Fill in default values
						dict [name] = MakeDefault (p.PropertyType);
					}		
				}

				// Call the saver
				string id = this.GetId ();
				var saver = ResourceMap [type];
				saver (id, dict);

				return new Dictionary<string, object> {
					{ "id", id }
				};
			}
		}
		// End class IdResourceBase

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

			public override string GetId ()
			{
				// Use <homework_id>/<user_id> as key of submission
				return String.Format ("{0}/{1}", this.Homework_id, (this?.Owner?.Id) ?? "");
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
			public List<Submission>  Submissions { get; set; }
		}
	}
	// End namespace Trivial

	public class UpdateAgent
	{
		private APIWrapper apiWrapper;

		public bool Verbose { get; set; }

		public static UpdateAgent TryGetUpdateAgent (string baseUrl, string userName, string password)
		{
			try {
				return new UpdateAgent (baseUrl, userName, password);
			} catch (APIWrapperException e) {
				Console.WriteLine ("Initialize UpdateAgent fail, check the configuration" +
				" for \"server\":\n {0}", e.ToUserString ()); // Error
				return null;
			}
			
		}

		public UpdateAgent (string baseUrl, string userName, string password)
		{
			apiWrapper = new APIWrapper (baseUrl, userName, password);
		}

		public bool UpdateAll ()
		{
			try {
				var profileStatus = UpdateProfile ();
				var coursesStatus = UpdateCourses ("all");
				return profileStatus && coursesStatus;
			} catch (APIWrapperException e) {
				Console.WriteLine ("Updating information fail:\n {0}", e.ToUserString ()); // Error
				return false;
			}
		}

		public bool UpdateNow ()
		{
			try {
				var profileStatus = UpdateProfile ();
				var coursesStatus = UpdateCourses ("now");
				return profileStatus && coursesStatus;
			} catch (APIWrapperException e) {
				Console.WriteLine ("Updating information fail:\n {0}", e.ToUserString ()); // Error
				return false;
			}
		}

		public bool UpdateCourses (string semester)
		{
			List<string> courseIds;
			bool status = true;
			bool attendedStatus = UpdateAttended (semester, out courseIds);

			if (attendedStatus) {
				foreach (var courseId in courseIds) {
					status = UpdateCourseHomeworks (courseId) && status;
					status = UpdateCourseFiles (courseId) && status;
					status = UpdateCourseAnnouncements (courseId) && status;
				}
			}
			return status;
		}

		public bool UpdateAttended (string semester, out List<string> courseIds)
		{
			string jsonString;
			var status = apiWrapper.GetAttended (semester, out jsonString);

			if (!status.IsScuccessStatusCode ()) {
				// Get attended information fail
				Console.WriteLine ("Failed updating attended courses for semester {0}: server return code {1}.",
					semester, status); // warning
				courseIds = null;
				return false;
			} else {
				var courses = JsonConvert.DeserializeObject<List<Trivial.Course>> (jsonString);
				foreach (var course in courses) {
					// Console.WriteLine (course.Id);
					course.Save ();
				}
				courseIds = courses.Select (c => c.Id).ToList ();
				return true;
			}
		}

		public bool UpdateProfile ()
		{
			string jsonString;
			var status = apiWrapper.GetProfile (out jsonString);

			if (!status.IsScuccessStatusCode ()) {
				// Get profile information fail, log if Verbose
				Console.WriteLine ("Failed updating profile: server return code {0}.", status); // warning
				return false;
			} else {
				// Fixme: error handling
				// Fixme: 个人profile需要单独存储， 现在这个调用其实并没有存储
				JsonConvert.DeserializeObject<Trivial.User> (jsonString).Save ();
				return true;
			}
		}

		public bool UpdateCourseHomeworks (string courseId)
		{
			string jsonString;
			var status = apiWrapper.GetHomeworks (courseId, out jsonString);

			if (!status.IsScuccessStatusCode ()) {
				Console.WriteLine ("Failed updating homeworks for course {0}: server return code {1}.", courseId, status); // warning
				return false;
			} else {
				var homeworks = JsonConvert.DeserializeObject<List<Trivial.Homework>> (jsonString);
				foreach (var homework in homeworks) {
					homework.Save ();
				}
				return true;
			}
		}

		public bool UpdateCourseFiles (string courseId)
		{
			string jsonString;
			var status = apiWrapper.GetFiles (courseId, out jsonString);

			if (!status.IsScuccessStatusCode ()) {
				Console.WriteLine ("Failed updating files for course {0}: server return code {1}.", courseId, status); // warning
				return false;
			} else {
				var files = JsonConvert.DeserializeObject<List<Trivial.File>> (jsonString);
				if (files != null) { // why null here
					foreach (var file in files) {
						file.Save ();
					}
				}
				return true;
			}
		}

		public bool UpdateCourseAnnouncements (string courseId)
		{
			string jsonString;
			var status = apiWrapper.GetAnnouncements (courseId, out jsonString);

			if (!status.IsScuccessStatusCode ()) {
				Console.WriteLine ("Failed updating announcements for course {0}: server return code {1}.", courseId, status); // warning
				return false;
			} else {
				var announcements = JsonConvert.DeserializeObject<List<Trivial.Announcement>> (jsonString);
				foreach (var ann in announcements) {
					ann.Save ();
				}
				return true;
			}
		}
	}
	// End class UpdateAgent
}