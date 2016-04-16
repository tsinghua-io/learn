﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class BasicUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }

    public class BasicMe: BasicUser, IResource
    {
        public string DocId()
        {
            return API.ProfileURL();
        }

        public const string RESOURCE_TYPE = "me";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public async Task Update()
        {
            Console.WriteLine("Updating profile.");

            var profile = await API.Profile();
            profile.Save();

            Console.WriteLine("Profile updated.");
        }
    }

    public class Me : BasicMe
    {
        public Dictionary<string, List<string>> AttendedIds { get; set; } = new Dictionary<string, List<string>>();

        public void SaveAttendedIds()
        {
            this.Set("AttendedIds", AttendedIds);
        }

        public Dictionary<string, List<Course>> Attended()
        {
            var attended = new Dictionary<string, List<Course>>();
            foreach (var pair in AttendedIds)
            {
                var list = new List<Course>();
                foreach (var id in pair.Value)
                    list.Add(Course.Get(id));
                attended[pair.Key] = list;
            }
            return attended;
        }

        public async Task UpdateAttended()
        {
            Console.WriteLine("Updating attended courses.");

            var attended = await API.Attended("all");
            
            AttendedIds.Clear();
            foreach (var course in attended)
            {
                if (!AttendedIds.ContainsKey(course.SemesterId))
                    AttendedIds[course.SemesterId] = new List<string>();
                AttendedIds[course.SemesterId].Add(course.Id);
                course.Save();
            }
            SaveAttendedIds();
            
            Console.WriteLine("Attended courses updated, {0} fetched.", attended.Count);
        }

        public async Task UpdateMaterials(string semesterId = null)
        {
            if (AttendedIds.Count == 0)
                return;

            var ids = new List<string>();
            foreach (var pair in AttendedIds)
            {
                if (semesterId == null || pair.Key == semesterId)
                    ids.AddRange(pair.Value);
            }
            Console.WriteLine("Updating course materials for {0}.", string.Join(", ", ids));

            var materials = await API.CoursesMaterials(ids);

            for (int i = 0; i < ids.Count; i++)
            {
                var course = Course.Get(ids[i]);

                course.AnnouncementIds.Clear();
                course.FileIds.Clear();
                course.AssignmentIds.Clear();
                foreach (var announcement in materials[i].Announcements)
                {
                    announcement.Save();
                    course.AnnouncementIds.Add(announcement.Id);
                }
                foreach (var file in materials[i].Files)
                {
                    file.Save();
                    course.FileIds.Add(file.Id);
                }
                foreach (var assignment in materials[i].Assignments)
                {
                    assignment.Save();
                    course.AssignmentIds.Add(assignment.Id);
                }

                course.SaveIds();
            }

            Console.WriteLine("Course materials for {0} updated.", string.Join(", ", ids));
        }

        public static Me Get()
        {
            return Database.Get<Me>(new BasicMe().DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}