using System;
using System.Net;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;


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

    [Verb("info", HelpText = "Get information of attending courses.")]
    public class InfoOptions 
    {
        [Option('a', "all", HelpText = "Get information of all courses.")]
        public bool All { get; set; }
    }

    [Verb("ls", HelpText = "Get list of attending courses.")]
    public class LsOptions
    {
        [Option('a', "all", HelpText = "Get list of all courses.")]
        public bool All { get; set; }
    }


    public class LearnConsole
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<
                UpdateOptions, AnnouncementOptions, FileOptions,
                HomeworkOptions, ProfileOptions, InfoOptions,
                LsOptions>(args).MapResult(
                    (UpdateOptions opts) => Update(opts),
                    (AnnouncementOptions opts) => Announcement(opts),
                    (FileOptions opts) => File(opts),
                    (HomeworkOptions opts) => Homework(opts),
                    (ProfileOptions opts) => Profile(opts),
                    (InfoOptions opts) => Info(opts),
                    (LsOptions opts) => Ls(opts),
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

        public static int Info(InfoOptions opts)
        {
            if (opts.All)
            {
                Console.WriteLine("Information of all courses.");
            }
            else
            {
                Console.WriteLine("Information of attending courses.");
            }
            return 0;
        }

        public static int Ls(LsOptions opts)
        {
            if (opts.All)
            {
                Console.WriteLine("List of all courses.");
            }
            else
            {
                Console.WriteLine("List of attending courses.");
            }
            return 0;
        }
    }
}
