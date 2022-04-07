using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class AirportRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public AirportRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AirportRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This Constructor should only use for testing purposes");
            }
        }
        public virtual async  Task<Airport> GetAirportById(int airportId)
        {
            if (airportId < 0)
            {
                Console.WriteLine($"Argument exception in GetAirportByID! Airport ID: {airportId}");
                throw new ArgumentException("Please provide a valid Id");
            };

            return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId)
                ?? throw new AirportNotfoundException();

        }
    }
}
