using System;
using System.Collections.Generic;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;

namespace Base
{
	public static class RestSharpExtensionMethods
	{

		public static bool IsScuccessStatusCode (this HttpStatusCode responseCode)
		{
			int numericResponse = (int)responseCode;
			return numericResponse >= 200 && numericResponse <= 399;
		}
	}

	public class APIWrapper
	{
		private const string profileUrl = "/users/me";
		private const string attendedUrl = "/users/me/attended";
		private const string homeworksUrl = "/courses/{courseId}/homeworks";
		private const string filesUrl = "/courses/{courseId}/files";
		private const string announcementsUrl = "/courses/{courseId}/announcements";

		private RestClient client;
		static public Dictionary<string, string> CommonHeaders = new Dictionary<string, string> () {
			{ "Accept", "application/json; charset=utf-8" },
			{ "Accept-Encoding", "gzip, identity" },
			{ "User-Agent", "learnAPIWrapper/1.0" }
		};

		/// <summary>
		/// Constructor of APIWrapper
		/// </summary>
		/// <param name="baseUrl">URL of the proxy server.</param>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		/// <exception cref="Base.APIWrapperException">Throw exception when baseUrl is ill-formatted.</exception>
		public APIWrapper (string baseUrl, string userName, string password)
		{
			try {
				client = new RestClient (baseUrl);
			} catch (Exception e) {
				throw new APIWrapperException ("Constructing web client failed.", e);
			}

			client.Authenticator = new HttpBasicAuthenticator (userName, password);
		}

		public HttpStatusCode GetProfile (out string jsonString)
		{ 
			return get (profileUrl, out jsonString);
		}


		public HttpStatusCode GetAttended (string semester, out string jsonString)
		{
			return get (attendedUrl, out jsonString, queries: new Dictionary<string, string> () { { "semester", semester } });
		}

		public HttpStatusCode GetHomeworks (string courseId, out string jsonString)
		{
			return get (homeworksUrl, out jsonString, new Dictionary<string, string> () { { "courseId", courseId } });
		}

		public HttpStatusCode GetFiles (string courseId, out string jsonString)
		{
			return get (filesUrl, out jsonString, new Dictionary<string, string> () { { "courseId", courseId } });
		}

		public HttpStatusCode GetAnnouncements (string courseId, out string jsonString)
		{
			return get (announcementsUrl, out jsonString, new Dictionary<string, string> () { { "courseId", courseId } });
		}

		/// <summary>
		/// Get the response from the proxy server.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="jsonString">The json string that the proxy server return.</param>
		/// <param name="segs">Segments in the URL.</param>
		/// <param name="queries">GET queries.</param>
		private HttpStatusCode get (string path, out string jsonString, 
		                            Dictionary<string, string> segs = null, Dictionary<string, string> queries = null)
		{
			var request = new RestRequest (path, Method.GET);

			if (segs != null) {
				foreach (var seg in segs) {
					request.AddUrlSegment (seg.Key, seg.Value.Trim ());
				}
			}

			if (queries != null) {
				foreach (var query in queries) {
					request.AddQueryParameter (query.Key, query.Value.Trim ());
				}
			}

			foreach (var header in CommonHeaders) {
				request.AddHeader (header.Key, header.Value);
			}

			IRestResponse response = client.Execute (request);

			// Handling errors
			if (response.ErrorException != null) {
				const string message = "Error retrieving response.  Check inner details for more info.";
				throw new APIWrapperException (message, response.ErrorException);
			}

			jsonString = response.Content;
			return response.StatusCode;
		}
	}
}
