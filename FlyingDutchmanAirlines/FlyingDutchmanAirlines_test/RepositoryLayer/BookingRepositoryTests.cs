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
    public class BookingRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private BookingRepository _repository;
        [TestInitialize]
        public async Task TestInitialize()
        {
           _context = TestTools.InitializeInMemoryContext("FlyingDutchman");
            _repository = new BookingRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]

        public async Task CreateBooking_Success()
        {
            await _repository.CreateBooking(1, 0);
            Booking book = _context.Bookings.First();
            Assert.IsNotNull(book);
            Assert.AreEqual(1, book.CustomerId);
            Assert.AreEqual(0, book.FlightNumber);
        }


        [TestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(-1, -1)]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateBooking_failue_InvalidInputs(int customerId, int flightNumber)
        {
            await _repository.CreateBooking(customerId, flightNumber);

        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotAddBookingToDatabaseException))]
        public async Task CreateBooking_failue_DatabaseError()
        {
            await _repository.CreateBooking(0, 1);

        }
    }
}
