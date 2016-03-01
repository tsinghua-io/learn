using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using CommandLine;
using CommandLine.Text;

using Base;

namespace LearnConsole
{
	public class CommonOptions {
		[Option('u', "user", Required = true, HelpText = "The username.")]
		public string User { get; set; }

		[Option('p', "password", Required = true, HelpText = "The password.")]
		public string Password { get; set; }

		[Option('s', "server", HelpText = "The server address.", Default = "http://localhost")]
		public string Server { get; set; }
	}

    [Verb("update", HelpText = "Update all data.")]
	public class UpdateOptions: CommonOptions {}

    [Verb("annc", HelpText = "Get all Announcements.")]
	public class AnnouncementOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get Announcements of specific course.")]
        public string Course { get; set; }
    }

    [Verb("file", HelpText = "Get all files.")]
    public class FileOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get files of specific course.")]
        public string Course { get; set; }
    }

    [Verb("hw", HelpText = "Get all homeworks.")]
    public class HomeworkOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get homeworks of specific course.")]
        public string Course { get; set; }
    }

    [Verb("profile", HelpText = "Get user profile.")]
    public class ProfileOptions: CommonOptions {}

    [Verb("attend", HelpText = "List of all courses.")]
    public class AttendOptions: CommonOptions
    {
    	[Option("semester", HelpText = "--semester=now: List of courses in this semester.")]
    	public string semester { get; set; }

    	[Option("detail", HelpText = "Detailed information of courses.")]
    	public bool detail { get; set; }
    }


    public class LearnConsole
    {
		public static APIWrapper GetApiWrapper (CommonOptions options) 
		{
			LogInfo ("Using proxy server", options.Server);
			return new APIWrapper (options.Server, options.User, options.Password);
		}

        public static int Main(string[] args)
        {
			return Parser.Default.ParseArguments<
                UpdateOptions, AnnouncementOptions, FileOptions,
				HomeworkOptions, ProfileOptions, AttendOptions> (args).MapResult (
				(UpdateOptions opts) => Update (opts),
				(AnnouncementOptions opts) => Announcement (opts),
				(FileOptions opts) => File (opts),
				(HomeworkOptions opts) => Homework (opts),
				(ProfileOptions opts) => Profile (opts),
				(AttendOptions opts) => Attend (opts),
				errs => 1);
        }

        public static int Update(UpdateOptions opts)
        {
            Console.WriteLine("Update command parsed.");
            return 0;
        }

        public static int Announcement(AnnouncementOptions opts)
        {
			string jsonString;
			var status = GetApiWrapper (opts).GetAnnoucements (opts.Course, out jsonString);
			LogInfo (String.Format("Annoucements of course: {0}", opts.Course), 
				String.Format("{0}: {1}", status, jsonString));
            return 0;
        }

        public static int File(FileOptions opts)
        {
			string jsonString;
			var status = GetApiWrapper (opts).GetFiles (opts.Course, out jsonString);
			LogInfo (String.Format("Files of course: {0}", opts.Course), 
				String.Format("{0}: {1}", status, jsonString));
			return 0;
        }

        public static int Homework(HomeworkOptions opts)
        {
			string jsonString;
			var status = GetApiWrapper (opts).GetHomeworks (opts.Course, out jsonString);
			LogInfo (String.Format("Homeworks of course: {0}", opts.Course), 
				String.Format("{0}: {1}", status, jsonString));
			return 0;
        }

        public static int Profile(ProfileOptions opts)
        {
			string jsonString;
			var status = GetApiWrapper (opts).GetProfile (out jsonString);
			LogInfo (String.Format("Profile of user {0}", opts.User), 
				String.Format("{0}: {1}", status, jsonString));
			return 0;
        }

        public static int Attend(AttendOptions opts)
        {
        	if (opts.semester == null)
        	{
        		if (opts.detail)
        		{
        			Console.WriteLine("Detailed information of all courses.");
        		}
        		else 
        		{
        			Console.WriteLine("List of all courses.");
        		}
        	}
        	else if (opts.semester == "now") 
        	{
        		if (opts.detail)
        		{
        			Console.WriteLine("Detailed information of all courses in this semester.");
        		}
        		else 
        		{
        			Console.WriteLine("List of all courses in this semester.");
        		}
        	}
        	return 0;
        }
			
		public static void LogInfo(string title, string content)
		{
			Console.WriteLine (title + ":");
			Console.WriteLine ("\t" + content);
		}

    }
}
