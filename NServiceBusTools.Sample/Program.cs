using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Unicast;

namespace NServiceBusTools.Sample
{
    public static class Program
    {
        public static void Run()
        {

            IBus bus = new UnicastBus();

            var order = new Order
            {
                Id = 1,
                Items = new List<OrderLine>
                {
                    new OrderLine
                    {
                        Quantity = 1,
                        Product = new Product
                        {
                            Name = "Sunglasses",
                            Price = 120.99m
                        }
                    },
                    new OrderLine
                    {
                        Quantity = 10,
                        Product = new Product
                        {
                            Name = "Trash bags",
                            Price = 5.99m
                        }
                    }
                }
            };


            bus.Send(order);

        }
    }
}