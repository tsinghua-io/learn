using System;
using System.Net;
using System.Collections.Generic;
using RestSharp;
using Mono.Options;

using Base;

namespace LearnConsole
{
	class ParserTest {
		static int verbosity;
		const string resourceHelpString = 
			"1. If \"-c\" option is specified: f/files for files; " +
			"a/announcements for announcements; " +
			"h/homeworks for homeworks. " +
			"\n2. Else, i/info for personal info; attending for attending courses; " +
			"attended for attended courses.";
		public static void Main (string[] args)
		{
			bool show_help = false;

			string userName = "";
			string password = "";
			string baseUrl = null;
			string courseId = null;
			string resource = null;

			var p = new OptionSet () {
				{ "v", "increase debug message verbosity",
					v => { if (v != null) ++verbosity; } },
				{ "h|help",  "show this message and exit", 
					v => show_help = v != null },
				{ "u|user=", "the login name to the server",
					v => userName = v },
				{ "p|pass=", "the login password to the server",
					v => password = v },
				{ "b|base=", "the base url of the proxy server",
					v => baseUrl = v }, // required
				{ "c|course=", "the course id",
					v => courseId = v },
				{ "r|resource=", "the resource type to fetch:\n" + resourceHelpString,
					v => resource = v }
			};

			List<string> extra;
			try {
				extra = p.Parse (args);
			}
			catch (OptionException e) {
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `greet --help' for more information.");
				return;
			}

			if (show_help) {
				ShowHelp (p);
				return;
			}

			// required args
			if (baseUrl == null) {
				throw new InvalidOperationException ("Missing required option -r=BASE_URL");
			}

			string jsonString;
			HttpStatusCode responseCode;

			APIWrapper apiWrapper = new APIWrapper (baseUrl, userName, password);

			Console.WriteLine (resource);
			Console.WriteLine (courseId);
			if (courseId == null) {
				// Default fetch the personal info
				switch (resource) {
				case "i":
				case "info":
					responseCode = apiWrapper.GetPersonalInfo (out jsonString);
					break;
				case "attending":
					responseCode = apiWrapper.GetAttending (out jsonString);
					break;
				case "atttended":
					responseCode = apiWrapper.GetAttended (out jsonString);
					break;
				default:
					Console.WriteLine (
						String.Format ("Error: Unrecognized resource type for user infos -- \"{0}\"\n\"-r\" option help:\n{1}",
							resource,
							resourceHelpString));
					return;
				}
			} else {
				switch (resource)
				{
				case "h":
				case "homeworks":
					responseCode = apiWrapper.GetHomeworks(courseId, out jsonString);
					break;
				case "f":
				case "files":
					responseCode = apiWrapper.GetFiles(courseId, out jsonString);
					break;
				case "a":
				case "announcements":
					responseCode = apiWrapper.GetAnnoucements(courseId, out jsonString);;
					break;
				default:
					Console.WriteLine (
						String.Format ("Error: Unrecognized resource type for course infos -- \"{0}\"\n\"-r\" option help:\n{1}",
							resource,
							resourceHelpString));
					return;
				}
				
			}
				
			Console.WriteLine (String.Format ("The response is:\n {0}: {1}\n", responseCode, jsonString));
		}

		static void ShowHelp (OptionSet p)
		{
			Console.WriteLine ("Usage:  Learn-Console.exe [OPTIONS]");
			Console.WriteLine ("Test the api wrapper.");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions (Console.Out);
		}

		static void Debug (string format, params object[] args)
		{
			if (verbosity > 0) {
				Console.Write ("# ");
				Console.WriteLine (format, args);
			}
		}
	}
}
