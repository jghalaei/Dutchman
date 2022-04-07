using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class FlightRepository
    {
        FlyingDutchmanAirlinesContext _context;

        public FlightRepository(FlyingDutchmanAirlinesContext context)
        {
            _context= context; 
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This Constructor should only be used for testing");
            }
        }

        public virtual Queue<Flight> GetFlights()
        {
            Queue<Flight> flights =new Queue<Flight>(); 
            foreach(Flight flight in _context.Flights)
            {
                flights.Enqueue(flight);
            }
            return flights;
        }
        public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
        {
            if (!flightNumber.IsPositive())
            {
                Console.WriteLine($"Could not find flight in GetFlightByFlightNumber!  flightNumber = { flightNumber}");
                throw new FlightNotFoundException();
            }
            return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ?? throw new FlightNotFoundException();

        }
    }
}
