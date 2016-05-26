using NServiceBus;

namespace NServiceBusTools.Sample
{
    public class OrderHandler : IHandleMessages<Order>
    {
        private IBus bus;

        public OrderHandler(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(Order message)
        {
            // check user status
            var userStatus = bus.Send(new CheckUserStatus()).Register<UserStatus>().Result;

            if (userStatus == UserStatus.Denied)
            {
                bus.Send(new GenerateOrderDeniedEmail {Order = message});
            }

            // first send confirmation email
            bus.Send(new GenerateOrderReceivedEmail
            {
                Order = message
            });

            // process order?
        }
    }
}