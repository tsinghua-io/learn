using System;

namespace Base
{
	public class LearnBaseException: ApplicationException
	{
		public LearnBaseException () {}

		public LearnBaseException (string message) : base(message) {}

		public LearnBaseException (string message, Exception inner) : base(message, inner) {}
	}

	public class APIWrapperException: LearnBaseException
	{
		public APIWrapperException () {}

		public APIWrapperException (string message) : base(message) {}

		public APIWrapperException (string message, Exception inner) : base(message, inner) {}
	}
}

