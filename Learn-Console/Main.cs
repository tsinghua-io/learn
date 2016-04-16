using System;
using System.Collections.Generic;
using CommandLine;
using LearnTsinghua.Models;
using LearnTsinghua.Services;

namespace LearnTsinghua.Terminal
{

    public class CommonOptions
    {
        [Option("detail", HelpText = "Detailed informations.")]
        public bool Detail { get; set; }
    }

    [Verb("config", HelpText = "Get/set/list configurations.")]
    public class ConfigOptions
    {
        [Option('l', "list", HelpText = "List all the configurations.")]
        public bool List { get; set; }

        [Value(0, MetaName = "NAME", HelpText = "The name of the configuration.")]
        public string Name { get; set; }

        [Value(1, MetaName = "VALUE", HelpText = "If exists, will be the new value of the configuration.")]
        public string Value { get; set; }
    }

    [Verb("update", HelpText = "Update data.")]
    public class UpdateOptions
    {
        // not implemented yet
        [Option('v', "verbose", HelpText = "Verbosely print out the update informations.")]
        public bool Verbose { get; set; }

        [Option('a', "all", HelpText = "Update all informations of all attended courses." +
            "If not set, update all informations of attending courses.")]
        public bool All { get; set; }
    }

    [Verb("profile", HelpText = "Get user profile.")]
    public class ProfileOptions
    {
    }

    [Verb("course", HelpText = "List of all courses.")]
    public class CourseOptions: CommonOptions
    {
        [Option("semester", HelpText = "--semester=now: List of courses in this semester.")]
        public string semester { get; set; }

        [Value(0, Required = false, MetaName = "course", HelpText = "Get info of specific course.")]
        public string Course { get; set; }
        
    }

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
    public class AssignmentOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get homeworks of specific course.")]
        public string Course { get; set; }
    }

    [Verb("reset", HelpText = "Reset the database.")]
    public class ResetOptions: CommonOptions
    {
    }


    public class App
    {
        /// <summary>Read password from console</summary>
        /// http://stackoverflow.com/questions/29201697/hide-replace-when-typing-a-password-c
        public static string ReadPassword(string prompt)
        {
            string password = "";
            Console.Write(prompt); // write prompt
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line
            Console.WriteLine();
            return password;
        }

        /// <summary>
        /// Learn-Console's Entry Point.
        /// </summary>
        /// <param name="args">Command line arguments splitted by the system.</param>
        public static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<
                    UpdateOptions, ProfileOptions, CourseOptions,
                    AnnouncementOptions, FileOptions, AssignmentOptions,
                    ConfigOptions, ResetOptions>(args).MapResult(
                    (UpdateOptions opts) => Update(opts),
                    (ProfileOptions opts) => Profile(opts),
                    (CourseOptions opts) => Course(opts),
                    (AnnouncementOptions opts) => Announcement(opts),
                    (FileOptions opts) => File(opts),
                    (AssignmentOptions opts) => Assignment(opts),
                    (ConfigOptions opts) => Config(opts),
                    (ResetOptions opts) => Reset(opts),
                    errs => 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);
                return 1;
            }
        }


        public static int Update(UpdateOptions opts)
        {
            var userId = AppConfig.Get().UserId;
            API.UserId = userId;
            API.Password = ReadPassword(string.Format("Password for {0}: ", userId));

            var me = Me.Get();
//            me.Update().Wait();
//            me.UpdateAllAttended().Wait();

//            var attended = me.Attended();
//            var tasks = new Task[attended.Count];
//            for (int i = 0; i < attended.Count; i++)
//            {
//                tasks[i] = attended[i].UpdateStuff();
//            }
//            Task.WaitAll(tasks);
//            Console.WriteLine(JObject.FromObject(API.CoursesAnnouncements(new List<string>{ "122205", "109148" })));
//            me.UpdateAttended().Wait();
            me.UpdateMaterials().Wait();

            return 0;
        }

        public static int Profile(ProfileOptions opts)
        {
            var profile = Me.Get();
            Console.WriteLine(profile);
            return 0;
        }

        public static int Course(CourseOptions opts)
        {
            var attended = Me.Get().Attended();
            foreach (var pair in attended)
            {
                Console.WriteLine(pair.Key);
                foreach (var course in pair.Value)
                    Console.WriteLine(course);
            }
            return 0;
        }

        public static int Announcement(AnnouncementOptions opts)
        {
//            var announcement = Ann
            return 0;
        }

        public static int File(FileOptions opts)
        {

            return 0;
        }

        public static int Assignment(AssignmentOptions opts)
        {

            return 0;
        }

        public static int Config(ConfigOptions opts)
        {
            var config = AppConfig.Get();
            if (opts.List)
            {
                Console.WriteLine(config);
            }
            else
            {
                config.Set(opts.Name, opts.Value);
            }
            return 0;
        }

        public static int Reset(ResetOptions opts)
        {
            Database.Reset();
            return 0;
        }
    }
}
