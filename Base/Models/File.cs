using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class BasicFile : IResource
    {
        // Identifiers.
        public string Id { get; set; }

        public string CourseId { get; set; }
        
        // Metadata.
        public BasicUser Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Category { get; set; } = new List<string>();
        
        // Content.
        public string Filename { get; set; }

        public int Size { get; set; }

        public string DownloadURL { get; set; }

        public string DocId()
        {
            return API.FileURL(CourseId, Id);
        }

        public const string RESOURCE_TYPE = "file";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public string SizeString()
        {
            double K = 1024;
            double M = K * K;
            double G = K * M;

            double num;

            num = Size / G;
            if (num >= 1.0)
                return string.Format("{0:F1} GB", num);

            num = Size / M;
            if (num >= 1.0)
                return string.Format("{0:F1} MB", num);

            num = Size / K;
            if (num >= 1.0)
                return string.Format("{0:F1} KB", num);

            return Size + " B";
        }
    }

    public class File : BasicFile
    {
        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

        public static File Get(string courseId, string id)
        {
            var file = new BasicFile{ Id = id, CourseId = courseId };
            return Database.GetExisting<File>(file.DocId());
        }
    }
}
