using System;

namespace NServiceBusTools.Sample
{
    public class SendEmail
    {
        public string To;
        public string From;
        public string Body;

        public override string ToString()
        {
            return $"To: {To}{Environment.NewLine}From: {From}{Environment.NewLine}{Body}";
        }
    }
}