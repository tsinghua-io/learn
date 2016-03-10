using System;
using System.Collections.Generic;
using CommandLine;
using System.Linq;

using Base;

namespace LearnConsole
{

	public class CommonOptions
	{
		[Option ("detail", HelpText = "Detailed informations.")]
		public bool detail { get; set; }
	}

	[Verb ("config", HelpText = "Get and set configurations.")]
	public class ConfigOptions{
		
	}

	[Verb ("update", HelpText = "Update all data.")]
	public class UpdateOptions
	{
		[Option ('u', "user", Required = true, HelpText = "The username.")]
		public string User { get; set; }

		[Option ('p', "password", Required = true, HelpText = "The password.")]
		public string Password { get; set; }

		[Option ('s', "server", HelpText = "The server address.", Default = "http://localhost")]
		public string Server { get; set; }

		[Option ('v', "verbose", HelpText = "Verbosely print out the update informations.")]
		public bool Verbose { get; set; }

		[Option ('a', "all", HelpText = "Update all informations of all courses.")]
		public bool All { get; set; }
	}

	[Verb ("annc", HelpText = "Get all Announcements.")]
	public class AnnouncementOptions: CommonOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get Announcements of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("file", HelpText = "Get all files.")]
	public class FileOptions: CommonOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get files of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("hw", HelpText = "Get all homeworks.")]
	public class HomeworkOptions: CommonOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get homeworks of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("info", HelpText = "Show course info.")]
	public class CourseInfoOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get info of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("profile", HelpText = "Get user profile.")]
	public class ProfileOptions
	{

	}

	[Verb ("attend", HelpText = "List of all courses.")]
	public class AttendOptions: CommonOptions
	{
		[Option ("semester", HelpText = "--semester=now: List of courses in this semester.")]
		public string semester { get; set; }

	}


	public class LearnConsole
	{
		public static APIWrapper GetApiWrapper (UpdateOptions options)
		{
			LogInfo ("Using proxy server", options.Server);
			return new APIWrapper (options.Server, options.User, options.Password);
		}

		public static UpdateAgent GetUpdateAgent (UpdateOptions options)
		{
			LogInfo ("Using proxy server", options.Server);
			return new UpdateAgent (options.Server, options.User, options.Password);
		}

		public static int Main (string[] args)
		{
			return Parser.Default.ParseArguments<
                UpdateOptions, AnnouncementOptions, FileOptions,
				HomeworkOptions, ProfileOptions, AttendOptions,
				CourseInfoOptions> (args).MapResult (
				(UpdateOptions opts) => Update (opts),
				(AnnouncementOptions opts) => Announcement (opts),
				(FileOptions opts) => File (opts),
				(HomeworkOptions opts) => Homework (opts),
				(ProfileOptions opts) => Profile (opts),
				(AttendOptions opts) => Attend (opts),
				(CourseInfoOptions opts) => Info (opts),
				errs => 1);
		}

		public static int Update (UpdateOptions opts)
		{
			Console.WriteLine ("Start updating");
			bool status;
			if (opts.All) {
				status = GetUpdateAgent (opts).UpdateAll ();
			} else {
				status = GetUpdateAgent (opts).UpdateNow ();
			}
			if (!status) {
				Console.WriteLine ("Finish updating, but some update failed.");
			} else {
				Console.WriteLine ("Finish updating, no errors");
			}
			return status ? 0 : 1;
		}

		public static int Announcement (AnnouncementOptions opts)
		{
			var view = Base.Announcement.database.GetView ("announcementsByCourseId");
			var query = view.CreateQuery ();
			query.Keys = new List<string> () { opts.Course };

			var annIds = query.Run ().Select (x => x.Value);
			var anns = annIds.Select (id => new Base.Announcement (
				new Dictionary<string, object> { { "id", id } })).ToList<Base.Announcement> ();

			if (!opts.detail) {
				anns.ForEach (a => Console.WriteLine ("{0,-10} | {1}", a.Id, a.Title));	
			} else {
				anns.ForEach (a => Console.WriteLine (a));
			}
			return 0;
		}

		public static int File (FileOptions opts)
		{
			var view = Base.File.database.GetView ("filesByCourseId");
			var query = view.CreateQuery ();
			query.Keys = new List<string> () { opts.Course };

			var fileIds = query.Run ().Select (x => x.Value);
			var files = fileIds.Select (id => new Base.File (
				new Dictionary<string, object> { { "id", id } })).ToList<Base.File> ();

			if (!opts.detail) {
				files.ForEach (f => Console.WriteLine ("{0,-10} | {1}", f.Id, f.Title));	
			} else {
				files.ForEach (f => Console.WriteLine (f));
			}
			return 0;
		}

		public static int Homework (HomeworkOptions opts)
		{
			var view = Base.Homework.database.GetView ("homeworksByCourseId");
			var query = view.CreateQuery ();
			query.Keys = new List<string> () { opts.Course };

			var homeworkIds = query.Run ().Select (x => x.Value);
			var homeworks = homeworkIds.Select (id => new Base.Homework (
				new Dictionary<string, object> { { "id", id } })).ToList<Base.Homework> ();

			if (!opts.detail) {
				homeworks.ForEach (h => Console.WriteLine ("{0,-10} | {1}", h.Id, h.Title));	
			} else {
				homeworks.ForEach (h => Console.WriteLine (h));
			}
			return 0;
		}

		public static int Info (CourseInfoOptions opts) 
		{
			try {
				var course = new Base.Course(new Dictionary<string, object>() {
					{"id", opts.Course}
				});
				Console.WriteLine (course);
				return 0;
			} catch (LearnBaseException ex) {
				Console.WriteLine ("Error fetching course info of id {0}: {1}", opts.Course, ex.Message);
				return 1;
			}
		}

		public static int Profile (ProfileOptions opts)
		{
			return 0;
		}
			
		public static int Attend (AttendOptions opts)
		{
			string semester = opts.semester ?? "all";
			List<Course> courses;

			switch (semester) {
			case "all":
				var view = Base.Course.database.GetView ("courseIds");
				var query = view.CreateQuery ();
				var courseIds = query.Run ().Select (x => x.Key);
				courses = courseIds.Select (id => new Base.Course (
					new Dictionary<string, object> { { "id", id } })).ToList<Base.Course> ();
				break;
			case "now":
				// Todo: get courses form now-course list
				courses = new List<Course> ();
				break;
			default:
				Console.WriteLine ("Unsupported semester argument \"{0}\".", semester);
				courses = new List<Course> ();
				break;
			}

			if (!opts.detail) {
				courses.ForEach (c => Console.WriteLine ("{0,-25}| {1,-50}{2,-20}", c.Id, c.Name, c.Semester));	
			} else {
				courses.ForEach(c => Console.WriteLine (c));
			}
			return 0;
		}

		public static void LogInfo (string title, string content)
		{
			Console.WriteLine (title + ":");
			Console.WriteLine ("\t" + content);
		}

	}
}
