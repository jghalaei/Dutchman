using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.EntityFrameworkCore;
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

    public class BookingServiceTests
    {
        private BookingService _service;
        private Mock<BookingRepository> _mockBookingRepository;
        private Mock<CustomerRepository> _mockCustomerRepository;
        private Mock<FlightRepository> _mockFlightRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingRepository = new();
            _mockCustomerRepository = new();
            _mockFlightRepository = new();

            _service = new BookingService(_mockBookingRepository.Object, _mockFlightRepository.Object, _mockCustomerRepository.Object);

        }

        [TestMethod]
        public async Task CreateBooking_success()
        {
            _mockBookingRepository.Setup(repo => repo.CreateBooking(1, 1))
                .Returns(Task.CompletedTask);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByName("Exist Customer"))
                .Returns(Task.FromResult(new Customer("Exist Customer") { CustomerId=1}));
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(new Flight { FlightNumber = 1 });

            (bool result, Exception? exception) = await _service.CreateBooking("Exist Customer", 1);
            Assert.IsTrue(result);
            Assert.IsNull(exception);
        }

        [TestMethod]
        public async Task CreateBooking_success_CustomerNotInDatabase()
        {
            _mockBookingRepository.Setup(repo => repo.CreateBooking(1, 1))
                .Returns(Task.CompletedTask);
            _mockCustomerRepository.SetupSequence(repo => repo.GetCustomerByName("NotExist Customer"))
                .Throws(new CustomerNotFoundException());
            _mockCustomerRepository.SetupSequence(repo => repo.GetCustomerByName("NotExist Customer"))
                .ReturnsAsync(new Customer("NotExist Customer") { CustomerId = 1 });
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(new Flight { FlightNumber = 1 });

            (bool result, Exception? exception) = await _service.CreateBooking("NotExist Customer", 1);
            Assert.IsTrue(result);
            Assert.IsNull(exception);

        }
        [TestMethod]
        public async Task CreateBooking_failure_CouldNotAddCustomer()
        {
            _mockBookingRepository.Setup(repo => repo.CreateBooking(1, 1))
                .Returns(Task.CompletedTask);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByName("NotExist Customer"))
                .Throws(new CustomerNotFoundException());
            _mockCustomerRepository.Setup(repo => repo.CreateCustomer("NotExist Customer")).Throws(new CouldNotAddCustomerToDatabase());
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(new Flight { FlightNumber = 1 });

            (bool result, Exception? exception) = await _service.CreateBooking("NotExist Customer", 1);
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);

        }

        [TestMethod]
        [DataRow("", 0)]
        [DataRow("Leo Tolstoy", -1)]
        [DataRow(null, -1)]

        public async Task CreateBooking_failure_InvalidArgument(string name, int flightName)
        {
            BookingService service = new BookingService(_mockBookingRepository.Object, _mockFlightRepository.Object, _mockCustomerRepository.Object);
            (bool result, Exception? exception) = await service.CreateBooking(name, flightName);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);

        }

        [TestMethod]
        public async Task CreateBooking_failure_ArgumentException()
        {
            _mockBookingRepository.Setup(repo => repo.CreateBooking(0, 1)).Throws(new ArgumentException());
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByName("Galileo Galilei"))
                .Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 }));
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(1)).ReturnsAsync(new Flight { FlightNumber = 1 });

            (bool result, Exception? exception) = await _service.CreateBooking("Galileo Galilei", 1);
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        }
        [TestMethod]
        public async Task CreateBooking_failure_RepositoryCouldNotAddBookingException()
        {
            _mockBookingRepository.Setup(repo => repo.CreateBooking(0, 2)).Throws(new CouldNotAddBookingToDatabaseException());

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByName("Galileo Galilei"))
                .Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 }));


            (bool result, Exception? exception) = await _service.CreateBooking("Galileo Galilei", 2);
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));


        }
        [TestMethod]
        public async Task CreateBooking_failure_FlightNotExist()
        {
            //_mockBookingRepository.Setup(repo => repo.CreateBooking(0, -1)).Returns(Task.CompletedTask);
            //_mockCustomerRepository.Setup(repo => repo.GetCustomerByName("Leo Tolstoy")).Returns(Task.FromResult(new Customer("Leo Tolstoy")));
            _mockFlightRepository.Setup(repo => repo.GetFlightByFlightNumber(-1)).Throws(new FlightNotFoundException());

            (bool result, Exception? exception) = await _service.CreateBooking("Leo Tolstoy", -1);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
        }
    }
}
