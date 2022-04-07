using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{

    public class CustomerRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public CustomerRepository(FlyingDutchmanAirlinesContext Context)
        {
            _context = Context;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public CustomerRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This Constructor should only be used for testing");
            }
        }
        public virtual async Task<bool> CreateCustomer(string name)
        {
            if (IsInvalidCustomerName(name))
            {
                return false;
            }
            try
            {
                Customer newCustomer = new Customer(name);
                using (_context)
                {
                    _context.Customers.Add(newCustomer);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw new CouldNotAddCustomerToDatabase();
            }
            return true;
        }

        public virtual async Task<Customer> GetCustomerByName(string name)
        {
            if (IsInvalidCustomerName(name))
            {
                throw new CustomerNotFoundException();
            }
            return await _context.Customers.FirstOrDefaultAsync(c => c.Name == name) ?? throw new CustomerNotFoundException();


        }
        private bool IsInvalidCustomerName(string name)
        {
            char[] forbiddenChars = { '!', '@', '#', '$', '%', '&', '*' };
            return string.IsNullOrEmpty(name) || name.Any(x => forbiddenChars.Contains(x));

        }
    }
}
