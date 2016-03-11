using System;
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
	}
}

