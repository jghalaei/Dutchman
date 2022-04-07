using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_test.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.RepositoryLayer
{
    [TestClass]
    public class AirportRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private AirportRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {

            _context = TestTools.InitializeInMemoryContext("FlyingDutchman");
            
            _repository = new AirportRepository(_context);
            Assert.IsNotNull(_repository);

            SortedList<string, Airport> airports = getAirportList();
           
            await _context.Airports.AddRangeAsync(airports.Values);
            await _context.SaveChangesAsync();

        }

        private SortedList<string, Airport> getAirportList()
        {
            SortedList<string, Airport> airports = new SortedList<string, Airport>
            {
                {
                    "GOH",
                    new Airport
                {
                    AirportId=0,
                    City="NUUK",
                    Iata= "GOH"
                }
                },
                {
                    "PHX",
                    new Airport{
                        AirportId=1,
                        City="Phoenix",
                        Iata="PHX"
                    }
                },
                {
                    "DDH",
                    new Airport
                    {
                        AirportId = 2,
                        City = "Bennington",
                        Iata = "DDH"
                    }
                },
                {
                    "RDU",
                    new Airport
                    {
                    AirportId = 3,
                    City = "Raleigh-Durham",
                    Iata = "RDU"
                    }
                }
            };
            return airports;
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]

        public async Task GetAirportById_success(int airportId)
        {
            Airport airport = await _repository.GetAirportById(airportId);
            Assert.IsNotNull(airport);
            Airport dbAirport = _context.Airports.First(a => a.AirportId == airportId);
            Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
            Assert.AreEqual(dbAirport.Iata, airport.Iata);
            Assert.AreEqual(dbAirport.City, airport.City);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAirportById_failure_InvalidArgument()
        {
            StringWriter outputStream = new StringWriter();
            try
            {
                Console.SetOut(outputStream);
                Airport airport = await _repository.GetAirportById(-1);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(outputStream.ToString().Contains("Argument exception in GetAirportByID! Airport ID: -1"));
                // throw;
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                outputStream.Dispose();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(AirportNotfoundException))]
        public async Task GetAirportById_failure_NotFound()
        {
            Airport airport = await _repository.GetAirportById(10);
        }
    }
}
