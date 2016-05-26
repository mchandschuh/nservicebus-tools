using System;
using NServiceBus;

namespace NServiceBusTools.Sample
{
    public class CheckUserStatusHandler : IHandleMessages<CheckUserStatus>
    {
        private readonly IBus bus;

        public CheckUserStatusHandler(IBus bus)
        {
            this.bus = bus;
        }
        public void Handle(CheckUserStatus message)
        {
            if (DateTime.UtcNow.Millisecond%7 == 0)
            {
                bus.Return(UserStatus.Denied);
            }
            else
            {
                bus.Return(UserStatus.Ok);
            }
        }
    }
}