using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class BookingRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;
        public BookingRepository(FlyingDutchmanAirlinesContext context)
        {
            this._context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public BookingRepository()
        {
           if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This Constructor should only be used for testing");            
            }
        }
        public virtual async Task CreateBooking(int customerId, int flightNumber)
        {
            //if (customerId < 0 || flightNumber < 0)
            if( !customerId.IsPositive() || !flightNumber.IsPositive() )
            {
                Console.WriteLine($"Argument exception in creating Booking! CustomerId:{customerId}, Flight Number: {flightNumber}");
                throw new ArgumentException("Invalid Arguments provided");
            }

            Booking booking = new Booking
            {
                CustomerId = customerId,
                FlightNumber = flightNumber
            };
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                Console.WriteLine($"Exception during database query: {ex.Message}");
                throw new CouldNotAddBookingToDatabaseException();
            }

        }

    }
}
