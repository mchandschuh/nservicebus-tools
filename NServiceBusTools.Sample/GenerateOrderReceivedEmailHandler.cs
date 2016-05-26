using System;
using System.Linq;
using NServiceBus;

namespace NServiceBusTools.Sample
{
    public class GenerateOrderReceivedEmailHandler : IHandleMessages<GenerateOrderReceivedEmail>
    {
        private readonly IBus bus;

        public GenerateOrderReceivedEmailHandler(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(GenerateOrderReceivedEmail message)
        {
            var items = string.Join(Environment.NewLine, message.Order.Items.Select(x => x.Quantity + " units of " + x.Product.Name));
            var email = new SendEmail
            {
                To = message.Order.User.Email,
                From = "sys@funproducts.xyz",
                Body = $"We have received order #{message.Order.Id} for the following items:{Environment.NewLine}{items}"
            };
            bus.Send(email);
        }
    }
}