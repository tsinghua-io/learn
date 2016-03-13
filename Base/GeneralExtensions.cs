using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
	public static class LearnGeneralExtensionMethods
	{
		public static IDictionary<string, object> Update (this IDictionary<string, object> original,
		                                                  IDictionary<string, object> newDict)
		{
			newDict.ToList ().ForEach (item => original [item.Key] = item.Value);
			return original;
		}

		public static string ToUserString (this ApplicationException e) 
		{
			var sb = new StringBuilder ();
			sb.AppendFormat ("{0}: {1}\n", e.GetType().ToString(), e.Message);
			var inner = e.InnerException;
			while (inner != null) {
				sb.AppendFormat ("---> {0}: {1}\n", inner.GetType ().ToString (), inner.Message);
				inner = inner.InnerException;
			}
			return sb.ToString ().AddTabEachLine ();
		}

		public static string AddTabEachLine (this string str)
		{
			return str.Trim().Split ('\n').Select (s => "\t" + s).Aggregate ((i, j) => i + "\n" + j) + "\n";
		}
	}

}

