using System;
using DirectScale.Disco.Extension;

namespace hughgrace.Models
{
    public class CancelRouteOrder
    {
        public string OrderId { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public string DistributorId { get; set; }
        public OrderType OrderType { get; set; }
        public double OrderTotal { get; set; }
        public string OrderCountry { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderStatus { get; set; }
        public string EventType { get; set; }
        public DateTime EventDateUtc { get; set; }

    }
}
