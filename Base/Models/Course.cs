using System;
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

        public List<Schedule> Schedules { get; set; }

        public List<User> Teachers { get; set; }

        public List<User> Assistants { get; set; }
    }

    public class LocalCourse: Course
    {
        public string Abbreviation { get; set; }

        // Do not show in the course list.
        public bool Hide { get; set; }

        // Do not disturb.
        public bool Quiet { get; set; }
    }
}
