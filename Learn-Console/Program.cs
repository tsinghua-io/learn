using System;
using System.Collections.Generic;
using CommandLine;

using Base;

namespace LearnConsole
{

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
	}

	[Verb ("annc", HelpText = "Get all Announcements.")]
	public class AnnouncementOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get Announcements of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("file", HelpText = "Get all files.")]
	public class FileOptions
	{
		[Value (0, Required = true, MetaName = "course", HelpText = "Get files of specific course.")]
		public string Course { get; set; }
	}

	[Verb ("hw", HelpText = "Get all homeworks.")]
	public class HomeworkOptions
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
	public class AttendOptions
	{
		[Option ("semester", HelpText = "--semester=now: List of courses in this semester.")]
		public string semester { get; set; }

		[Option ("detail", HelpText = "Detailed information of courses.")]
		public bool detail { get; set; }
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
			Console.WriteLine ("Update command parsed.");
			var status = GetUpdateAgent (opts).UpdateAll ();
			if (!status) {
				Console.WriteLine ("Some update failed.");
			}
			return status ? 0 : 1;
		}

		public static int Announcement (AnnouncementOptions opts)
		{
			// Todo: Get announcements from announcements by courseId view
			return 0;
		}

		public static int File (FileOptions opts)
		{
			// Todo: Get files from files by courseId view
			return 0;
		}

		public static int Homework (HomeworkOptions opts)
		{
			// Todo: Get homeworks from homeworks by courseId view
			// for test, use course as homework id now
			/*var homework = new Homework (new Dictionary<string, object> () {
				{ "id", opts.Course }
			});
			Console.WriteLine (homework);*/
			return 0;
		}

		public static int Info (CourseInfoOptions opts) 
		{
			try {
				var course = new Course(new Dictionary<string, object>() {
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
			List<Course> courses = new List<Course> (); // just for compiling now

			switch (semester) {
			case "all":
				// Todo: get courses from all-course query
				break;
			case "now":
				// Todo: get courses form now-course query
				break;
			default:
				Console.WriteLine ("Unsupported semester argument \"{0}\".", semester);
				break;
			}

			if (opts.detail) {
				// All informations
			} else {
				// Only name and id are printed
				courses.ForEach(course => Console.WriteLine (course));
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
