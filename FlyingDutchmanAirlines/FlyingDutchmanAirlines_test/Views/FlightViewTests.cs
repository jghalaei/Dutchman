using FlyingDutchmanAirlines.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.Views
{
    [TestClass]
    public class FlightViewTests
    {
        [TestMethod]
        public void Constructor_FlightView_success()
        {
            FlightView flight = new FlightView("1", ("Tehran", "TEH"), ("Arlanda", "ARN"));
            Assert.IsNotNull(flight);
            Assert.AreEqual("1",flight.FlightNumber);
            Assert.AreEqual("Tehran", flight.Origin.City);
            Assert.AreEqual("TEH",flight.Origin.Code);
            Assert.AreEqual("Arlanda", flight.Destination.City);
            Assert.AreEqual("ARN", flight.Destination.Code);
        }
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Constructor_FlightView_success_FlightNumber_Empty(string flightNumber)
        {
            FlightView flight = new FlightView(flightNumber, ("Tehran", "TEH"), ("Arlanda", "ARN"));
            Assert.IsNotNull(flight);
            Assert.AreEqual("No flight number found", flight.FlightNumber);
            Assert.AreEqual("Tehran", flight.Origin.City);
            Assert.AreEqual("TEH", flight.Origin.Code);
            Assert.AreEqual("Arlanda", flight.Destination.City);
            Assert.AreEqual("ARN", flight.Destination.Code);


        }

        [TestMethod]
        public void Constructor_AirportInfo_Success_City_EmptyString()
        {
            string destinationCity = string.Empty;
            string destinationCityCode = "SYD";

            AirportInfo airportInfo = new AirportInfo((destinationCity, destinationCityCode));
            Assert.IsNotNull(airportInfo);

            Assert.AreEqual(airportInfo.City, "No city found");
            Assert.AreEqual(airportInfo.Code, destinationCityCode);
        }

        [TestMethod]
        public void Constructor_AirportInfo_Success_Code_EmptyString()
        {
            string destinationCity = "Ushuaia";
            string destinationCityCode = string.Empty;

            AirportInfo airportInfo = new AirportInfo((destinationCity, destinationCityCode));
            Assert.IsNotNull(airportInfo);

            Assert.AreEqual(airportInfo.City, destinationCity);
            Assert.AreEqual(airportInfo.Code, "No code found");
        }
    }
}
