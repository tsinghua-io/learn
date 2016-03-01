using System;
using System.Collections.Generic;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;

namespace Base
{
	public static class RestSharpExtensionMethods
	{

		public static bool IsScuccessStatusCode(this HttpStatusCode responseCode)
		{
			int numericResponse = (int)responseCode;
			return numericResponse >= 200
				&& numericResponse <= 399;
		}
	}

	public class APIWrapper
	{
		private const string profileUrl = "/users/me";
		private const string attendingUrl = "/users/me/attending";
		private const string attendedUrl = "/users/me/attended";
		private const string homeworksUrl = "/courses/{courseId}/homeworks";
		private const string filesUrl = "/courses/{courseId}/files";
		private const string announcementsUrl = "/courses/{courseId}/announcements";

		private RestClient client;
		public Dictionary<string, string> CommonHeaders { get; set; } = new Dictionary<string, string>()
		{
			{ "Accept", "application/json; charset=utf-8" },
			{ "Accept-Encoding", "gzip, identity" },
			{ "User-Agent", "learnAPIWrapper/1.0" }
		};

		public APIWrapper (string baseUrl, string userName, string password)
		{
			client = new RestClient (baseUrl);
			client.Authenticator = new HttpBasicAuthenticator (userName, password);

		}

		public HttpStatusCode GetProfile (out string jsonString) =>
			get (profileUrl, out jsonString);


		public HttpStatusCode GetAttending (out string jsonString) =>
			get (attendingUrl, out jsonString);


		public HttpStatusCode GetAttended (out string jsonString) =>
			get (attendedUrl, out jsonString);

		public HttpStatusCode GetHomeworks (string courseId, out string jsonString) =>
			get (homeworksUrl, out jsonString, new Dictionary<string, string>() {{"courseId", courseId}});

		public HttpStatusCode GetFiles (string courseId, out string jsonString) =>
			get (filesUrl, out jsonString, new Dictionary<string, string>() {{"courseId", courseId}});

		public HttpStatusCode GetAnnoucements (string courseId, out string jsonString) =>
			get (announcementsUrl, out jsonString, new Dictionary<string, string>() {{"courseId", courseId}});

		private HttpStatusCode get (string path, out string jsonString, 
			Dictionary<string, string> segs = null) {
			var request = new RestRequest(path, Method.GET);

			if (segs != null) {
				foreach (var seg in segs) {
					request.AddUrlSegment (seg.Key, seg.Value.Trim ());
				}
			}

			foreach (var header in CommonHeaders) {
				request.AddHeader (header.Key, header.Value);
			}

			IRestResponse response = client.Execute(request);

			// Handling errors
			if (response.ErrorException != null)
			{
				const string message = "Error retrieving response.  Check inner details for more info.";
				throw new ApplicationException(message, response.ErrorException);
			}

			jsonString = response.Content;
			return response.StatusCode;
		}
	}
}

