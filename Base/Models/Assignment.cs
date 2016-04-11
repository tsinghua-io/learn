using System;

namespace LearnTsinghua.Models
{
    public class Attachment
    {
        public string Filename { get; set; }

        public int Size { get; set; }

        public string DownloadURL { get; set; }

    }

    public class Submission
    {
        // Metadata.
        public User Owner { get; set; }

        public string AssignmentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Late { get; set; }


        // Content.
        public string Body { get; set; }

        public Attachment Attachment { get; set; }


        // Marking metadata.
        public User MarkedBy { get; set; }

        public DateTime MarkedAt { get; set; }


        // Marking content.
        public float? Mark { get; set; }

        public string Comment { get; set; }

        public Attachment CommentAttachment { get; set; }
    }

    public class Assignment
    {
        // Identifiers.
        public string Id { get; set; }

        public string CourseId { get; set; }


        // Metadata.
        public DateTime CreatedAt { get; set; }

        public DateTime BeginAt { get; set; }

        public DateTime DueAt { get; set; }


        // Content.
        public string Title { get; set; }

        public string Body { get; set; }

        public Attachment Attachment { get; set; }

        public Submission Submission { get; set; }
    }

    public class LocalAssignment: Assignment
    {
        // Mark as done, not matter what.
        public bool Done { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
