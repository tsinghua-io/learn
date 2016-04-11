using System;
using System.Collections.Generic;
using RestSharp.Authenticators;
using RestSharp;
using LearnTsinghua.Models;

namespace LearnTsinghua.Services
{
    // Representation of remote API.
    public interface IAPI
    {
        User Profile();

        IList<Course> AllAttended();

        IList<Announcement> CourseAnnouncements(string courseId);

        IList<File> CourseFiles(string courseId);

        IList<Assignment> CourseAssignments(string courseId);
    }

    public class API: IAPI
    {
        const string BASE_URL = "";
        const string VERSION = "0.0";
        const string USER_AGENT = "LearnTsinghuaCSharp/" + VERSION;

        const string PROFILE_URL = "/users/me";
        const string ATTENDED_URL = "/users/me/attended";
        const string COURSE_ANNOUNCEMENTS_URL = "/courses/{id}/announcements";
        const string COURSE_FILES_URL = "/courses/{id}/files";
        const string COURSE_ASSIGNMENTS_URL = "/courses/{id}/assignments";

        string userId;
        string password;
        string langCode;

        public API(string userId, string password, string langCode = "zh-CN")
        {
            this.userId = userId;
            this.password = password;
            this.langCode = langCode;
        }

        T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BASE_URL);
            client.Authenticator = new HttpBasicAuthenticator(userId, password);

            request.AddHeader("Accept", "application/json; charset=utf-8");
            request.AddHeader("Accept-Encoding", "gzip");
            request.AddHeader("User-Agent", USER_AGENT);

            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
                throw new ApplicationException("Failed to get response.", response.ErrorException);

            return response.Data;
        }

        public User Profile()
        {
            var request = new RestRequest(PROFILE_URL);
            return Execute<User>(request);
        }

        public IList<Course> AllAttended()
        {
            var request = new RestRequest(ATTENDED_URL);
            request.AddParameter("semester", "all");
            return Execute<IList<Course>>(request);
        }

        public IList<Announcement> CourseAnnouncements(string courseId)
        {
            var request = new RestRequest(COURSE_ANNOUNCEMENTS_URL);
            request.AddUrlSegment("id", courseId);
            return Execute<IList<Announcement>>(request);
        }

        public IList<File> CourseFiles(string courseId)
        {
            var request = new RestRequest(COURSE_FILES_URL);
            request.AddUrlSegment("id", courseId);
            return Execute<IList<File>>(request);
        }

        public IList<Assignment> CourseAssignments(string courseId)
        {
            var request = new RestRequest(COURSE_ASSIGNMENTS_URL);
            request.AddUrlSegment("id", courseId);
            return Execute<IList<Assignment>>(request);
        }
    }
}
