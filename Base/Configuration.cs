using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Couchbase.Lite;

namespace Base
{
	public class Configuration
	{
		private const string databaseName = "tsinghua-io-learn-configuration";
		private const string docId = "config";

		private static Database database = Globals.manager.GetDatabase(databaseName);
		private static Document doc = database.GetDocument(docId);

		// Configurations
		public string Username => (string)(doc.GetProperty("username")??"");
		public string Password => (string)(doc.GetProperty("password")??"");
		public string Server => (string)(doc.GetProperty("server")??"");

		public static List<string> ALL_CONFIG = new List<string>() {"Username", "Password", "Server"};
		public static List<string> REQUIRED_CONFIG = new List<string>() {"Username", "Password", "Server"};

		public List<string> LackConfig => REQUIRED_CONFIG
			.Where(cfgName =>
				(string)(this.GetType().GetProperty(cfgName).GetValue(this)) == "")
			.ToList();

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			var type = this.GetType();
			foreach (var cfgName in ALL_CONFIG) {
				var p = type.GetProperty(cfgName);
				var name = p.Name;
				var value = p.GetValue (this);
				if (value != null) {
					sb.AppendFormat ("{0,-10}: {1}\n", name, value.ToString());
				}
			}
			return sb.ToString ();    
		}

		public static bool UpdateConfiguration (Dictionary<string, object> vals) {
			doc.Update((UnsavedRevision newRevision) =>
				{
					var properties = newRevision.Properties;
					properties.Update(vals);
					return true;
				});
			return true;
		}
	}
}

