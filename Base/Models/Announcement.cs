using System;

namespace LearnTsinghua.Models
{
    public class Announcement
    {
        // Identifiers.
        public string Id { get; set; }

        public string CourseId { get; set; }


        // Metadata.
        public User Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Priority { get; set; }


        // Content.
        public string Title { get; set; }

        public string Body { get; set; }
    }
}
