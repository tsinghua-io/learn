using System;
using System.Collections.Generic;

namespace Base
{
	public class User
    {        
		public string id;
		public string name;
		public string type;
		public string department;
		public string @class;
		public string gender;
		public string email;
		public string phone;
    }

    public class TimeLocation
    {
        public string weeks;
        public int day_of_week;
        public int period_of_day;
        public string location;
    }

    public class Course
    {
        // Identifiers.
        public string id;
        public string semester;
        public string course_number;
        public string course_sequence;

        // Metadata.
        public string name;
        public int credit;
        public int hour;
        public string description;

        // Time & location.
        public TimeLocation [] time_locations;

        // Staff.
		public User [] teachers;
		public User [] assistants;
    }

    public class Announcement
    {
        // Identifiers.
        public string id;
        public string course_id;

        // Metadata.
		public User owner;
        public string created_at;
        public int priority;
        public bool read;

        // Content.
        public string title;
        public string body;
    }

    public class File
    {
        // Identifiers.
        public string id;
        public string course_id;

        // Metadata.
		public User owner;
        public string created_at;
        public string title;
        public string description;
        public string [] category;
        public bool read;

        // Content.
        public string filename;
        public int size;
        public string download_url;
    }

    public class Attachment
    {
        public string filename;
        public int size;
        public string download_url;
    }

    public class Submission 
    {
        // Metadata.
		public User owner;
        public string created_at;
        public bool late;

        // Content.
        public string body;
		public Attachment attachment;

        // Scoring metadata.
		public User marked_by;
        public string marked_at;

        // Scoring content.
		public double mark;
        public string comment;
		public Attachment comment_attachment;
    }

    public class Homework
    {
        // Identifiers.
        public string id;
        public string course_id;

        // Metadata.
        public string created_at;
        public string begin_at;
        public string due_at;
        public int submitted_count;
        public int not_submitted_count;
        public int seen_count;
        public int marked_count;

        // Content.
        public string title;
        public string body;
		public Attachment attachment;

        // Submissions.
		public Submission [] submissions;
    }
}

