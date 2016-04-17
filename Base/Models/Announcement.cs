using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using AngleSharp.Parser.Html;
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
        public static Announcement Get(string courseId, string id)
        {
            var announcement = new BasicAnnouncement{ Id = id, CourseId = courseId };
            return Database.Get<Announcement>(announcement.DocId());
        }

        public string BodyText()
        {
            var doc = new HtmlParser().Parse(Body);
            return Regex.Replace(doc.DocumentElement.TextContent.Trim(), @"\s+", " ");
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
