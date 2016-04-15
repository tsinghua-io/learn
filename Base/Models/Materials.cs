using System.Collections.Generic;

namespace LearnTsinghua.Models
{
    public class Materials
    {
        public List<Announcement> Announcements { get; set; }

        public List<File> Files { get; set; }

        public List<Assignment> Assignments { get; set; }
    }
}
