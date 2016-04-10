using System.Collections.Generic;

namespace LearnTsinghua.Models
{
    public class Schedule
    {
        public string Weeks { get; set; }

        public int Day { get; set; }

        public int Slot { get; set; }

        public string Location { get; set; }
    }

    public class Course
    {
        public Course()
        {
            Schedules = new List<Schedule>();
            Teachers = new List<User>();
            Assistants = new List<User>();
        }

        // Identifires.
        public string Id { get; set; }

        public string Semester { get; set; }

        public string Number { get; set; }

        public string Sequence { get; set; }


        // Metadata.
        public string Name { get; set; }

        public int Credit { get; set; }

        public int Hour { get; set; }

        public string Description { get; set; }

        public IList<Schedule> Schedules { get; set; }

        public IList<User> Teachers { get; set; }

        public IList<User> Assistants { get; set; }
    }
}
