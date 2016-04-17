using System;
using System.Collections.Generic;
using CommandLine;
using LearnTsinghua.Extensions;
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

    [Verb("semester", HelpText = "Get current semester.")]
    public class SemesterOptions
    {
    }

    [Verb("course", HelpText = "List of all courses.")]
    public class CourseOptions: CommonOptions
    {
        [Option('a', "all", HelpText = "")]
        public bool All { get; set; }
    }

    [Verb("annc", HelpText = "Get all Announcements.")]
    public class AnnouncementOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get Announcements of specific course.")]
        public string Course { get; set; }

        [Value(1, MetaName = "index", HelpText = "Get a specific announcement.")]
        public int? Index { get; set; }
    }

    [Verb("file", HelpText = "Get all files.")]
    public class FileOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get files of specific course.")]
        public string Course { get; set; }

        [Value(1, MetaName = "index", HelpText = "Get a specific file.")]
        public int? Index { get; set; }
    }

    [Verb("hw", HelpText = "Get all homeworks.")]
    public class AssignmentOptions: CommonOptions
    {
        [Value(0, Required = true, MetaName = "course", HelpText = "Get homeworks of specific course.")]
        public string Course { get; set; }

        [Value(1, MetaName = "index", HelpText = "Get a specific homework.")]
        public int? Index { get; set; }
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
                    UpdateOptions, ProfileOptions, SemesterOptions, CourseOptions,
                    AnnouncementOptions, FileOptions, AssignmentOptions,
                    ConfigOptions, ResetOptions>(args).MapResult(
                    (UpdateOptions opts) => UpdateHandler(opts),
                    (ProfileOptions opts) => ProfileHandler(opts),
                    (SemesterOptions opts) => SemesterHandler(opts),
                    (CourseOptions opts) => CourseHandler(opts),
                    (AnnouncementOptions opts) => AnnouncementHandler(opts),
                    (FileOptions opts) => FileHandler(opts),
                    (AssignmentOptions opts) => AssignmentHandler(opts),
                    (ConfigOptions opts) => ConfigHandler(opts),
                    (ResetOptions opts) => ResetHandler(opts),
                    errs => 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);
                return 1;
            }
        }

        public static int UpdateHandler(UpdateOptions opts)
        {
            var userId = AppConfig.Get().UserId;
            API.UserId = userId;
            API.Password = ReadPassword(string.Format("Password for {0}: ", userId));

            if (opts.All)
            {
                Semester.Update().Wait();
                Me.Update().Wait();
                Me.UpdateAttended().Wait();
                Me.UpdateMaterials().Wait();
            }
            else
            {
                Me.UpdateMaterials(Semester.Get().Id).Wait();
            }

            return 0;
        }

        public static int ProfileHandler(ProfileOptions opts)
        {
            var profile = Me.Get();
            Console.WriteLine(profile);
            return 0;
        }

        public static int SemesterHandler(SemesterOptions opts)
        {
            var semester = Semester.Get();
            Console.WriteLine("{0} 第{1}周", Semester.IdToString(semester.Id), semester.WeekNow());
            return 0;
        }

        public static int CourseHandler(CourseOptions opts)
        {
            var attended = Me.Get().Attended(opts.All ? null : Semester.Get().Id);
            foreach (var pair in attended)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("{0} ({1}门课)", Semester.IdToString(pair.Key), pair.Value.Count);

                foreach (var course in pair.Value)
                {
                    Console.ResetColor();
                    Console.Write(course.Name);
                    if (course.Schedules.Count > 0)
                    {
                        var location = course.Schedules[0].Location;
                        if (!string.IsNullOrEmpty(location))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(" {0}", location);
                        }
                            
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        foreach (var schedule in course.Schedules)
                            Console.Write(" {0}-{1} ({2})", schedule.Day, schedule.Slot, schedule.Weeks);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            return 0;
        }

        public static int AnnouncementHandler(AnnouncementOptions opts)
        {
            var course = Course.FuzzyGet(opts.Course);
            if (course == null)
                return 1;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("{0} ({1})", course.Name, Semester.IdToString(course.SemesterId));

            var anncs = course.Announcements();

            if (opts.Index != null)
            {
                if (opts.Index <= 0 || opts.Index > anncs.Count)
                    return 1;
                var annc = anncs[(int)opts.Index - 1];

                Console.ForegroundColor = annc.Priority >= 1 ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
                Console.WriteLine(annc.Title);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("{0} {1}", annc.Owner?.Name, annc.CreatedAt);

                Console.ResetColor();
                Utils.WriteWithHighlights(annc.BodyText().Oneliner());
                Console.WriteLine();
            }
            else
            {
                var index = 0;
                foreach (var annc in anncs)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("{0,6}", annc.CreatedAt.DaysSince());
                    
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" {0,3}", ++index);
                    
                    Console.ForegroundColor = annc.Priority >= 1 ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
                    Console.WriteLine(" {0}", annc.Title);
                }
                Console.ResetColor();
            }
            return 0;
        }

        public static int FileHandler(FileOptions opts)
        {
            var course = Course.FuzzyGet(opts.Course);
            if (course == null)
                return 1;
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("{0} ({1})", course.Name, Semester.IdToString(course.SemesterId));

            var files = course.Files();

            if (opts.Index != null)
            {
                if (opts.Index <= 0 || opts.Index > files.Count)
                    return 1;
                var file = files[(int)opts.Index - 1];

                // TODO: Download file.
            }
            else
            {
                var index = 0;
                foreach (var file in files)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("{0,6}", file.CreatedAt.DaysSince());
                    
                    Console.Write(" ");
                    Utils.WriteFileSize(file.Size);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" {0,3}", ++index);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(" {0}", file.Title);
                    
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(" {0}", file.Description.Oneliner());
                }
                Console.ResetColor();
            }
            return 0;
        }

        public static int AssignmentHandler(AssignmentOptions opts)
        {
            var course = Course.FuzzyGet(opts.Course);
            if (course == null)
                return 1;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("{0} ({1})", course.Name, Semester.IdToString(course.SemesterId));

            var assignments = course.Assignments();

            if (opts.Index != null)
            {
                if (opts.Index <= 0 || opts.Index > assignments.Count)
                    return 1;
                var assignment = assignments[(int)opts.Index - 1];
            }
            else
            {
                var index = 0;
                foreach (var assignment in assignments)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("{0,5}天", (assignment.DueAt - DateTime.Now).Days);
                   
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" {0,3}", ++index);

                    Console.ForegroundColor = assignment.Submission != null ?
                        ConsoleColor.DarkGray :
                        (DateTime.Now > assignment.DueAt ?
                            ConsoleColor.DarkRed :
                            ConsoleColor.DarkYellow);
                    Console.WriteLine(" {0}", assignment.Title);
                }
                Console.ResetColor();
            }
            return 0;
        }

        public static int ConfigHandler(ConfigOptions opts)
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

        public static int ResetHandler(ResetOptions opts)
        {
            Database.Reset();
            return 0;
        }
    }
}
