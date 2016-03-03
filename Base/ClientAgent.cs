using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Base
{
	public class ClientAgent
	{
		private APIWrapper apiWrapper;

		public ClientAgent (string baseUrl, string userName, string password)
		{
			apiWrapper = new APIWrapper (baseUrl, userName, password);
		}

		public User GetProfile ()
		{
			string jsonString;
			var status = apiWrapper.GetProfile (out jsonString);
			if (!status.IsScuccessStatusCode ()) {
				// Get profile information fail
				return null;
			} else {
				return JsonConvert.DeserializeObject<User> (jsonString);
			}
		}

		public List<Course> GetAttended (string semester)
		{
			string jsonString;
			var status = apiWrapper.GetAttended (semester, out jsonString);
			if (!status.IsScuccessStatusCode ()) {
				// Get profile information fail
				return null;
			} else {
				return JsonConvert.DeserializeObject<List<Course>> (jsonString);
			}
		}
	}
}

