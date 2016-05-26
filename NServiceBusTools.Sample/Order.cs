using System.Collections.Generic;

namespace NServiceBusTools.Sample
{
    public class Order
    {
        public int Id;
        public User User;
        public List<OrderLine> Items;
    }
}