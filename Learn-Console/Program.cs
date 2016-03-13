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

	[Verb ("config", HelpText = "Get/set/list configurations.")]
	public class ConfigOptions
	{
		[Option ('l', "list", HelpText = "List all the configurations.")]
		public bool List { get; set; }

		[Value (0, MetaName = "NAME", HelpText = "The name of the configuration.")]
		public string Name { get; set; }

		[Value (1, MetaName = "VALUE", HelpText = "If exists, will be the new value of the configuration.")]
		public string Value { get; set; }
	}

	[Verb ("update", HelpText = "Update data.")]
	public class UpdateOptions
	{
		// not implemented yet
		[Option ('v', "verbose", HelpText = "Verbosely print out the update informations.")]
		public bool Verbose { get; set; }

		[Option ('a', "all", HelpText = "Update all informations of all attended courses." +
		"If not set, update all informations of attending courses.")]
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
		public static UpdateAgent GetUpdateAgent ()
		{
			var cfg = new Base.Configuration ();
			var lackConfig = cfg.LackConfig;

			if (lackConfig.Count == 0) {
				var password = ReadPassword ("Password for tsinghua's learning website: ");
				return UpdateAgent.TryGetUpdateAgent (cfg.Server, cfg.Username, password);
			} else {
				// Configuration is not complete
				Console.WriteLine ("Configuration is not complete: lack configuration: {0}.",
					String.Join (", ", lackConfig));
				return null;
			}
		}

		/// <summary>Read password from console</summary>
		/// http://stackoverflow.com/questions/29201697/hide-replace-when-typing-a-password-c
		public static string ReadPassword (string prompt)
		{
			string password = "";
			Console.Write (prompt); // write prompt
			ConsoleKeyInfo info = Console.ReadKey (true);
			while (info.Key != ConsoleKey.Enter) {
				if (info.Key != ConsoleKey.Backspace) {
					password += info.KeyChar;
				} else if (info.Key == ConsoleKey.Backspace) {
					if (!string.IsNullOrEmpty (password)) {
						// remove one character from the list of password characters
						password = password.Substring (0, password.Length - 1);
					}
				}
				info = Console.ReadKey (true);
			}
			// add a new line
			Console.WriteLine ();
			return password;
		}

		/// <summary>
		/// Learn-Console's Entry Point.
		/// </summary>
		/// <param name="args">Command line arguments splitted by the system.</param>
		public static int Main (string[] args)
		{
			return Parser.Default.ParseArguments<
                UpdateOptions, AnnouncementOptions, FileOptions,
				HomeworkOptions, ProfileOptions, AttendOptions,
				CourseInfoOptions, ConfigOptions> (args).MapResult (
				(UpdateOptions opts) => Update (opts),
				(AnnouncementOptions opts) => Announcement (opts),
				(FileOptions opts) => File (opts),
				(HomeworkOptions opts) => Homework (opts),
				(ProfileOptions opts) => Profile (opts),
				(AttendOptions opts) => Attend (opts),
				(CourseInfoOptions opts) => Info (opts),
				(ConfigOptions opts) => Config (opts),
				errs => 1);
		}

		public static int Config (ConfigOptions opts)
		{
			if (opts.List) {
				// List configuration
				var cfg = new Base.Configuration ();
				Console.WriteLine (cfg);
			} else {
				if (opts.Name == null) {
					Console.WriteLine ("Wrong arguments, see \"--help\" for help.");
				} else {
					if (opts.Value == null) {
						// Get configuration
						Console.WriteLine (Base.Configuration.GetConfiguration (opts.Name));
					} else {
						// Set configuration
						var updateDict = new Dictionary<string, object> () {
							{ opts.Name, opts.Value }
						};
						Base.Configuration.UpdateConfiguration (updateDict);
					}
				}

			}
			return 0;
		}

		public static int Update (UpdateOptions opts)
		{
			Console.WriteLine ("Start updating...");
			var updateAgent = GetUpdateAgent ();
			if (updateAgent == null) {
				return 1;
			}

			bool status;
			if (opts.All) {
				status = updateAgent.UpdateAll ();
			} else {
				status = updateAgent.UpdateNow ();
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
				Console.WriteLine (anns.ToStr ());
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
				Console.WriteLine (files.ToStr ());
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
				Console.WriteLine (homeworks.ToStr ());
			}
			return 0;
		}

		public static int Info (CourseInfoOptions opts)
		{
			try {
				var course = new Base.Course (new Dictionary<string, object> () {
					{ "id", opts.Course }
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
				Console.WriteLine (courses.ToStr ());
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
