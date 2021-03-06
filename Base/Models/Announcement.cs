﻿using System;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Extensions;
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

        private string bodyText;

        public string BodyText()
        {
            return bodyText ?? (bodyText = Body.RemoveTags());
        }
    }

    public class Announcement : BasicAnnouncement
    {
        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }


        public static Announcement Get(string courseId, string id)
        {
            var announcement = new BasicAnnouncement{ Id = id, CourseId = courseId };
            return Database.Get<Announcement>(announcement.DocId());
        }
    }
}
