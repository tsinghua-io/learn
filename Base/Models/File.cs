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
    }

    public class File : BasicFile
    {
        public static File Get(string courseId, string id)
        {
            var file = new BasicFile{ Id = id, CourseId = courseId };
            return Database.GetExisting<File>(file.DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

        public static string SizeToString(int size)
        {
            double K = 1024;
            double M = K * K;
            double G = K * M;

            double num;

            num = size / G;
            if (num >= 10.0)
                return string.Format("{0:F0}G", num);
            else if (num >= 1.0)
                return string.Format("{0:F1}G", num);

            num = size / M;
            if (num >= 10.0)
                return string.Format("{0:F0}M", num);
            else if (num >= 1.0)
                return string.Format("{0:F1}M", num);
            
            num = size / K;
            if (num >= 10.0)
                return string.Format("{0:F0}K", num);
            else if (num >= 1.0)
                return string.Format("{0:F1}K", num);

            return size + "B";
        }
    }
}
