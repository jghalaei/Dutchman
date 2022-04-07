using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    public sealed class Customer
    {
        public Customer(string name)
        {
            Bookings = new HashSet<Booking>();
            Name = name;
        }

        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; }

        public static bool operator == (Customer x, Customer y)
        {
            CustomerEqualityComparer comparer = new CustomerEqualityComparer();
            return comparer.Equals(x, y);
        }

        public static bool operator !=(Customer x, Customer y) => !(x == y);

    }
}
