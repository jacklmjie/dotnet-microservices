﻿using System;

namespace User.API.Infrastructure.Exceptions
{
    public class UserDomainException : Exception
    {
        public UserDomainException() { }

        public UserDomainException(string message) : base(message) { }

        public UserDomainException(string message, Exception innerExpection) : base(message, innerExpection) { }
    }
}
