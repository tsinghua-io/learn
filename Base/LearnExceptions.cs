using System;

namespace Base
{
	public class LearnBaseException: Exception
	{
		public LearnBaseException () {}

		public LearnBaseException (string message) : base(message) {}

		public LearnBaseException (string message, Exception inner) : base(message, inner) {}
	}
}

