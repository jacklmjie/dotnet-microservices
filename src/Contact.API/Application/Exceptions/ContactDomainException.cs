using System;

namespace User.API.Application.Exceptions
{
    public class ContactDomainException : Exception
    {
        public ContactDomainException() { }

        public ContactDomainException(string message) : base(message) { }

        public ContactDomainException(string message, Exception innerExpection) : base(message, innerExpection) { }
    }
}
