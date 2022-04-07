using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_test.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.RepositoryLayer
{
    [TestClass]
    public class FlightRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private FlightRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _context = TestTools.InitializeInMemoryContext("FlyingDutchman");
            _repository = new FlightRepository(_context);
            Assert.IsNotNull(_repository);

            Flight flight = new Flight
            {
                FlightNumber = 1,
                Origin = 1,
                Destination = 2
            };
            Flight flight2 = new Flight
            {
                FlightNumber = 10,
                Origin = 3,
                Destination = 4
            };
            _context.Flights.Add(flight);
            _context.Flights.Add(flight2);
            await _context.SaveChangesAsync();

        }


        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_InvalidFlightNumber()
        {
            await _repository.GetFlightByFlightNumber(-1);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            Flight flight = await _repository.GetFlightByFlightNumber(1);
            Assert.IsNotNull(flight);

            Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
            Assert.IsNotNull(dbFlight);

            Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
            Assert.AreEqual(dbFlight.Origin, flight.Origin);
            Assert.AreEqual(dbFlight.Destination, flight.Destination);
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_DatabaseException()
        {
            await _repository.GetFlightByFlightNumber(2);
        }

        [TestMethod]
        public async Task GetFlights_success()
        {
            Queue<Flight> flights = _repository.GetFlights();
            Assert.IsNotNull(flights);
            Assert.AreEqual(flights.Count, 2);
        }
    }
}
