using System;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class BasicAnnouncement : IResource
    {
        // Identifiers.
        public string Id { get; set; }

        public string CourseId { get; set; }

        // Metadata.
        public BasicUser Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Priority { get; set; }
        
        // Content.
        public string Title { get; set; }

        public string Body { get; set; }

        public string DocId()
        {
            return API.AnnouncementURL(CourseId, Id);
        }

        public const string RESOURCE_TYPE = "announcement";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }
    }

    public class Announcement : BasicAnnouncement
    {
        public Announcement(string courseId, string id)
        {
            CourseId = courseId;
            Id = id;
        }

        public static Announcement Get(string courseId, string id)
        {
            return Database.GetExisting<Announcement>(new Announcement(courseId, id).DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
