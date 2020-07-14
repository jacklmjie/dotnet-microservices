using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Services
{
    public class MessageSmsService : IMessageService
    {
        public string Send()
        {
            return "Sms";
        }
    }
}
