using System;

namespace LearnTsinghua
{
    public class LearnTsinghuaException: ApplicationException
    {
        public LearnTsinghuaException()
        {
        }

        public LearnTsinghuaException(string message)
            : base(message)
        {
        }

        public LearnTsinghuaException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class APIException: LearnTsinghuaException
    {
        public APIException()
        {
        }

        public APIException(string message)
            : base(message)
        {
        }

        public APIException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
