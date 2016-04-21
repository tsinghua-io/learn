using System;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Extensions;
using LearnTsinghua.Services;

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
        public BasicUser Owner { get; set; }

        public string AssignmentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Late { get; set; }


        // Content.
        public string Body { get; set; }

        public Attachment Attachment { get; set; }


        // Marking metadata.
        public BasicUser MarkedBy { get; set; }

        public DateTime MarkedAt { get; set; }


        // Marking content.
        public float? Mark { get; set; }

        public string Comment { get; set; }

        public Attachment CommentAttachment { get; set; }

        private string bodyText;

        public string BodyText()
        {
            return bodyText ?? (bodyText = Body.RemoveTags());
        }

        private string commentText;

        public string CommentText()
        {
            return commentText ?? (commentText = Comment.RemoveTags());
        }
    }

    public class BasicAssignment : IResource
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

        public string DocId()
        {
            return API.AssignmentURL(CourseId, Id);
        }

        public const string RESOURCE_TYPE = "assignment";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public string BodyText()
        {
            return Body.RemoveTags();
        }
    }

    public class Assignment: BasicAssignment
    {
        public bool MarkAsDone { get; set; }

        public void SaveConfig()
        {
            this.Set("MarkAsDone", MarkAsDone);
        }

        public bool Done()
        {
            return Submission != null || MarkAsDone;
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }


        public static Assignment Get(string courseId, string id)
        {
            var assignment = new BasicAssignment { Id = id, CourseId = courseId };
            return Database.Get<Assignment>(assignment.DocId());
        }
    }
}
