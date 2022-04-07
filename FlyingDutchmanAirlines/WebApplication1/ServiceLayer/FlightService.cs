using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.ServiceLayer
{
    public class FlightService
    {
        private FlightRepository _flightRepository;
        private AirportRepository _airportRepository;

        public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightService()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This Constructor should only use for testing purposes");
            }
        }

        public virtual async IAsyncEnumerable<FlightView> GetFlights()
        {
            Queue<Flight> flights = _flightRepository.GetFlights();
            foreach (Flight flight in flights)
            {
                Airport originAirport;
                Airport destinationAirport;
                try
                {
                    originAirport = await _airportRepository.GetAirportById(flight.Origin);
                    destinationAirport = await _airportRepository.GetAirportById(flight.Destination);
                }
                catch (AirportNotfoundException)
                {
                    throw new AirportNotfoundException();
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }
                yield return new FlightView(flight.FlightNumber.ToString(),
                (originAirport.City, originAirport.Iata),
                (destinationAirport.City, destinationAirport.Iata));
            }


        }

        public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
        {
            Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);
            Airport origin = await _airportRepository.GetAirportById(flight.Origin);
            Airport destination = await _airportRepository.GetAirportById(flight.Destination);
            
            return new FlightView(flight.FlightNumber.ToString(),(origin.City, origin.Iata),(destination.City, destination.Iata));
        }
    }
}
