using NServiceBus;

namespace NServiceBusTools.Sample
{
    public class GenerateOrderDeniedEmailHandler : IHandleMessages<GenerateOrderDeniedEmail>
    {
        private readonly IBus bus;

        public GenerateOrderDeniedEmailHandler(IBus bus)
        {
            this.bus = bus;
        }
        public void Handle(GenerateOrderDeniedEmail message)
        {
            var email = new SendEmail
            {
                To = message.Order.User.Email,
                From = "sys@funproducts.xyz",
                Body = $"Order #{message.Order.Id} was denied due to user status"
            };
            bus.Send(email);
        }
    }
}