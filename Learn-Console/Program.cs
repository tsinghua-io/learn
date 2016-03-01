using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using CommandLine;
using CommandLine.Text;

using Base;

namespace LearnConsole
{
    [Verb("update", HelpText = "Update all data.")]
    public class UpdateOptions {}

    [Verb("annc", HelpText = "Get all Announcements.")]
    public class AnnouncementOptions 
    {
        [Value(0, MetaName = "course", HelpText = "Get Announcements of specific course.")]
        public string Course { get; set; }
    }

    [Verb("file", HelpText = "Get all files.")]
    public class FileOptions
    {
        [Value(0, MetaName = "course", HelpText = "Get files of specific course.")]
        public string Course { get; set; }
    }

    [Verb("hw", HelpText = "Get all homeworks.")]
    public class HomeworkOptions
    {
        [Value(0, MetaName = "course", HelpText = "Get homeworks of specific course.")]
        public string Course { get; set; }
    }

    [Verb("profile", HelpText = "Get user profile.")]
    public class ProfileOptions {}

    // [Verb("info", HelpText = "Get information of attending courses.")]
    // public class InfoOptions 
    // {
    //     [Option('a', "all", HelpText = "Get information of all courses.")]
    //     public bool All { get; set; }
    // }

    // [Verb("ls", HelpText = "Get list of attending courses.")]
    // public class LsOptions
    // {
    //     [Option('a', "all", HelpText = "Get list of all courses.")]
    //     public bool All { get; set; }
    // }

    [Verb("attend", HelpText = "List of all courses.")]
    public class AttendOptions
    {
    	[Option("semester", HelpText = "--semester=now: List of courses in this semester.")]
    	public string semester { get; set; }

    	[Option("detail", HelpText = "Detailed information of courses.")]
    	public bool detail { get; set; }
    }

    // public class Options
    // {
    // 	[Value(0)]
    // 	public string Course { get; set; }
    // }


    public class LearnConsole
    {
        public static int Main(string[] args)
        {
//			var url = "/user/123";
//			var json = "{\"doc_id\":\"/user/123\",\"id\":\"123\",\"name\":\"gyl\",\"type\":\"undergraduate\",\"department\n\":\"ME\",\"class\":\"ME32\",\"gender\":\"male\",\"email\":\"sample@gmail.com\",\"phone\":\"188123\n456789\"}";
//			var me = new User (url, json);
//			Console.WriteLine (JsonConvert.SerializeObject (me));
//			me.@class = "EE36";
//			Console.WriteLine (JsonConvert.SerializeObject (me));
            return Parser.Default.ParseArguments<
                UpdateOptions, AnnouncementOptions, FileOptions,
				HomeworkOptions, ProfileOptions, AttendOptions>(args).MapResult(
                    (UpdateOptions opts) => Update(opts),
                    (AnnouncementOptions opts) => Announcement(opts),
                    (FileOptions opts) => File(opts),
                    (HomeworkOptions opts) => Homework(opts),
                    (ProfileOptions opts) => Profile(opts),
                    (AttendOptions opts) => Attend(opts),
                    // (InfoOptions opts) => Info(opts),
                    // (LsOptions opts) => Ls(opts),
                    // (Options opts) => News(opts),
                    errs => 1);
        }

        public static int Update(UpdateOptions opts)
        {
            Console.WriteLine("Update command parsed.");
            return 0;
        }

        public static int Announcement(AnnouncementOptions opts)
        {
            if (opts.Course == null)
            {
                Console.WriteLine("Announcements of course: all");
            }
            else
            {
                Console.WriteLine("Announcements of course: {0}", opts.Course);

            }
            return 0;
        }

        public static int File(FileOptions opts)
        {
            if (opts.Course == null)
            {
                Console.WriteLine("Files of course: all");
            }
            else
            {
                Console.WriteLine("Files of course: {0}", opts.Course);
            }
            return 0;
        }

        public static int Homework(HomeworkOptions opts)
        {
            if (opts.Course == null)
            {
                Console.WriteLine("Homeworks of course: all");
            }
            else
            {
                Console.WriteLine("Homeworks of course: {0}", opts.Course);
            }
            return 0;
        }

        public static int Profile(ProfileOptions opts)
        {
            Console.WriteLine("Profile command parsed.");
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

        // public static int Info(InfoOptions opts)
        // {
        //     if (opts.All)
        //     {
        //         Console.WriteLine("Information of all courses.");
        //     }
        //     else
        //     {
        //         Console.WriteLine("Information of attending courses.");
        //     }
        //     return 0;
        // }

        // public static int Ls(LsOptions opts)
        // {
        //     if (opts.All)
        //     {
        //         Console.WriteLine("List of all courses.");
        //     }
        //     else
        //     {
        //         Console.WriteLine("List of attending courses.");
        //     }
        //     return 0;
        // }

        // public static int News(Options opts)
        // {
        // 	if (opts.Course == null)
        // 	{
        // 		Console.WriteLine("News of course: all");
        // 	}
        // 	else 
        // 	{
        // 		Console.WriteLine("News of course: {0}", opts.Course);
        // 	}
        // 	return 0;
        // }
    }
}
