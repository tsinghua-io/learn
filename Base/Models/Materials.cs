using System.Collections.Generic;

namespace LearnTsinghua.Models
{
    public class Materials
    {
        public List<Announcement> Announcements { get; set; } = new List<Announcement>();

        public List<File> Files { get; set; } = new List<File>();

        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
