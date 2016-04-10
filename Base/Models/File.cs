using System;
using System.Collections.Generic;

namespace LearnTsinghua.Models
{
    public class File
    {
        public File()
        {
            Category = new List<string>();
        }

        // Identifiers.
        public string Id { get; set; }

        public string CourseId { get; set; }


        // Metadata.
        public User Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IList<string> Category { get; set; }


        // Content.
        public string Filename { get; set; }

        public int Size { get; set; }

        public string DownloadURL { get; set; }
    }
}
