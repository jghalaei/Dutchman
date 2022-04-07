using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.ServiceLayer
{


    [TestClass]
    public class FlightServiceTests
    {
        private Mock<FlightRepository> _mockFlightRepository;
        private Mock<AirportRepository> _mockAirportRepository;
        FlightService _service;
        [TestInitialize]
        public void TestInit()
        {
            _mockFlightRepository = new();
            _mockAirportRepository = new();

            _mockAirportRepository.Setup(repository => repository.GetAirportById(1)).ReturnsAsync(new Airport
            {
                AirportId = 1,
                City = "Mexico City",
                Iata = "MEX"
            });
            _mockAirportRepository.Setup(repository => repository.GetAirportById(2)).ReturnsAsync(new Airport
            {
                AirportId = 2,
                City = "Ulaanbaataar",
                Iata = "UBN"
            });
            _mockAirportRepository.Setup(repository => repository.GetAirportById(3)).Throws(new AirportNotfoundException());
            _service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
        }
        [TestMethod]
        public async Task GetFlights_Success()
        {
            Flight flightInDatabase = new Flight
            {
                FlightNumber = 148,
                Origin = 1,
                Destination = 2
            };

            Queue<Flight> mockReturn = new Queue<Flight>(1);
            mockReturn.Enqueue(flightInDatabase);

            _mockFlightRepository.Setup(repository => repository.GetFlights()).Returns(mockReturn);
            await foreach (FlightView flightView in _service.GetFlights())
            {
                Assert.IsNotNull(flightView);
                Assert.AreEqual(flightView.FlightNumber, "148");
                Assert.AreEqual(flightView.Origin.City, "Mexico City");
                Assert.AreEqual(flightView.Origin.Code, "MEX");
                Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
                Assert.AreEqual(flightView.Destination.Code, "UBN");
            }
        }

        [TestMethod]
        public void GetFlights_success_NoFlight()
        {
            Queue<Flight> mockReturn = new Queue<Flight>();

            _mockFlightRepository.Setup(repository => repository.GetFlights()).Returns(mockReturn);
            IAsyncEnumerable<FlightView> flights = _service.GetFlights();
            Assert.IsNotNull(flights);

        }


        [TestMethod]
        [ExpectedException(typeof(AirportNotfoundException))]
        public async Task GetFlights_failure_AirportNotFound()
        {
            Flight flightInDatabase = new Flight
            {
                FlightNumber = 148,
                Origin = 3,
                Destination = 2
            };

            Queue<Flight> mockReturn = new Queue<Flight>(1);
            mockReturn.Enqueue(flightInDatabase);

            _mockFlightRepository.Setup(repository => repository.GetFlights()).Returns(mockReturn);
            await foreach (FlightView flightView in _service.GetFlights()) { };
        }

        [TestMethod]
        public async Task GetFlighByFlightNumber_success()
        {
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1)).ReturnsAsync(new Flight() { FlightNumber = 1, Origin = 1, Destination = 2 });
            FlightView view = await _service.GetFlightByFlightNumber(1);

            Assert.IsNotNull(view);
            Assert.AreEqual(view.FlightNumber, "1");
            Assert.AreEqual(view.Origin.City, "Mexico City");
            Assert.AreEqual(view.Origin.Code, "MEX");
            Assert.AreEqual(view.Destination.City, "Ulaanbaataar");
            Assert.AreEqual(view.Destination.Code, "UBN");
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlighByFlightNumber_failure_FlightNotFound()
        {
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(-1)).Throws(new FlightNotFoundException());
            FlightView view = await _service.GetFlightByFlightNumber(-1);

        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotfoundException))]
        public async Task GetFlighByFlightNumber_failure_AirportNotFound()
        {
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1)).ReturnsAsync(new Flight() { FlightNumber = 1, Origin = 3, Destination = 2 });
            FlightView view = await _service.GetFlightByFlightNumber(1);

        }

    }
}