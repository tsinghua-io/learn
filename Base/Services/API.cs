using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp.Authenticators;
using RestSharp;
using LearnTsinghua;
using LearnTsinghua.Models;

namespace LearnTsinghua.Services
{
    public static class API
    {
        const string VERSION = "0.0";
        const string USER_AGENT = "LearnTsinghuaCSharp/" + VERSION;

        public static string BaseURL { get; set; }

        public static string UserId { get; set; }

        public static string Password { get; set; }

        public static string LangCode { get; set; }

        static API()
        {
            BaseURL = "https://api.tsinghua.io";
            LangCode = "zh-CN";
        }

        public static string SemesterURL()
        {
            return "semester";
        }

        public static string ProfileURL()
        {
            return "users/me";
        }

        public static string AttendedURL()
        {
            return ProfileURL() + "/attended";
        }

        public static string CourseURL(string id)
        {
            return "courses/" + id;
        }

        public static string CourseAnnouncementsURL(string courseId)
        {
            return CourseURL(courseId) + "/announcements";
        }

        public static string CourseFilesURL(string courseId)
        {
            return CourseURL(courseId) + "/files";
        }

        public static string CourseAssignmentsURL(string courseId)
        {
            return CourseURL(courseId) + "/assignments";
        }

        public static string CourseMaterialsURL(string courseId)
        {
            return CourseURL(courseId) + "/materials";
        }

        public static string AnnouncementURL(string courseId, string id)
        {
            return CourseAnnouncementsURL(courseId) + "/" + id;
        }

        public static string FileURL(string courseId, string id)
        {
            return CourseFilesURL(courseId) + "/" + id;
        }

        public static string AssignmentURL(string courseId, string id)
        {
            return CourseAssignmentsURL(courseId) + "/" + id;
        }

        public static string AuthorizationsURL()
        {
            return "authorizations";
        }

        public static async Task<T> ExecuteAsync<T>(IRestRequest request)
        {
            var client = new RestClient(BaseURL);
            client.Authenticator = new HttpBasicAuthenticator(UserId, Password);

            request.AddHeader("Accept", "application/json; charset=utf-8");
            request.AddHeader("Accept-Encoding", "gzip");
            request.AddHeader("User-Agent", USER_AGENT);
            var url = client.BaseUrl + request.Resource;

            var response = await client.ExecuteTaskAsync<T>(request);
            if (response.ErrorException != null)
                throw new APIException(string.Format("Failed to get response from {0}.", url), response.ErrorException);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new APIException(string.Format("{0} got from {1}: {2}.", response.StatusCode, url, response.Content));

            return response.Data;
        }

        public static async Task<BasicSemester> Semester()
        {
            var request = new RestRequest(SemesterURL());
            return await ExecuteAsync<BasicSemester>(request);
        }

        public static async Task<BasicMe> Profile()
        {
            var request = new RestRequest(ProfileURL());
            return await ExecuteAsync<BasicMe>(request);
        }

        public static async Task<List<BasicCourse>> Attended(string semester)
        {
            var request = new RestRequest(AttendedURL());
            request.AddParameter("semester", semester);
            return await ExecuteAsync<List<BasicCourse>>(request);
        }

        public static async Task<List<BasicAnnouncement>> CourseAnnouncements(string courseId)
        {
            var request = new RestRequest(CourseAnnouncementsURL(courseId));
            return await ExecuteAsync<List<BasicAnnouncement>>(request);
        }

        public static async Task<List<List<BasicAnnouncement>>> CoursesAnnouncements(IList<string> courseId)
        {
            var request = new RestRequest(CourseAnnouncementsURL(string.Join(",", courseId)));
            return await ExecuteAsync<List<List<BasicAnnouncement>>>(request);
        }

        public static async Task<List<BasicFile>> CourseFiles(string courseId)
        {
            var request = new RestRequest(CourseFilesURL(courseId));
            return await ExecuteAsync<List<BasicFile>>(request);
        }

        public static async Task<List<List<BasicFile>>> CoursesFiles(IList<string> courseId)
        {
            var request = new RestRequest(CourseFilesURL(string.Join(",", courseId)));
            return await ExecuteAsync<List<List<BasicFile>>>(request);
        }

        public static async Task<List<BasicAssignment>> CourseAssignments(string courseId)
        {
            var request = new RestRequest(CourseAssignmentsURL(courseId));
            return await ExecuteAsync<List<BasicAssignment>>(request);
        }

        public static async Task<List<List<BasicAssignment>>> CoursesAssignments(IList<string> courseId)
        {
            var request = new RestRequest(CourseAssignmentsURL(string.Join(",", courseId)));
            return await ExecuteAsync<List<List<BasicAssignment>>>(request);
        }

        public static async Task<Materials> CourseMaterials(string courseId)
        {
            var request = new RestRequest(CourseMaterialsURL(courseId));
            return await ExecuteAsync<Materials>(request);
        }

        public static async Task<List<Materials>> CoursesMaterials(IList<string> courseId)
        {
            var request = new RestRequest(CourseMaterialsURL(string.Join(",", courseId)));
            return await ExecuteAsync<List<Materials>>(request);
        }

        public static async Task<Dictionary<string, string>> Authorizations(string url)
        {
            var request = new RestRequest(AuthorizationsURL());
            request.AddParameter("url", url);
            return await ExecuteAsync<Dictionary<string, string>>(request);
        }
    }
}
