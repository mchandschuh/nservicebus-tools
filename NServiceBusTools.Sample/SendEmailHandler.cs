using System;
using NServiceBus;

namespace NServiceBusTools.Sample
{
    public class SendEmailHandler : IHandleMessages<SendEmail>
    {
        public void Handle(SendEmail message)
        {
            Console.WriteLine("SendEmailHandler");
            Console.WriteLine(message);
        }
    }
}