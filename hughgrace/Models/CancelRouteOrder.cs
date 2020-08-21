using System;
using DirectScale.Disco.Extension;

namespace hughgrace.Models
{
    public class CancelRouteOrder
    {
        public int OrderId { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public int DistributorId { get; set; }
        public OrderType OrderType { get; set; }
        public double OrderTotal { get; set; }
        public string OrderContry { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderStatus { get; set; }
        public string EventType { get; set; }
        public DateTime EventDataUtc { get; set; }

    }
}
