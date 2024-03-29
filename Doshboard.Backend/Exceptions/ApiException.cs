﻿namespace Doshboard.Backend.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base()
        {
        }

        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
