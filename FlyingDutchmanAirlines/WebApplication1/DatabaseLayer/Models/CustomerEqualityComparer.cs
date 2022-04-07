using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    internal class CustomerEqualityComparer : EqualityComparer<Customer>
    {
        public override bool Equals(Customer? x, Customer? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            return x.CustomerId == y.CustomerId && x.Name == y.Name;
        }

        public override int GetHashCode([DisallowNull] Customer obj)
        {
            int randomNumber = RandomNumberGenerator.GetInt32(int.MaxValue / 2);
            return obj.CustomerId + obj.Name.Length + randomNumber;
        }
    }
}
