using FlyingDutchmanAirlines.Controllers;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.Controllers
{
    [TestClass]
    public class FlightControllerTests
    {
        private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(List<FlightView> returnFlightViews)
        {
            foreach (FlightView flight in returnFlightViews)
            {
                yield return flight;
            }
        }

        [TestMethod]
        public async Task GetFlights_success()
        {
            Mock<FlightService> service = new Mock<FlightService>();

            List<FlightView> returnFlightViews = new List<FlightView>(2)
            {
                new FlightView("1",("Tehran","10"),("Arlanda","20")),
                new FlightView("2",("Istanbul","12"),("Paris","22"))
            };
            service.Setup(s => s.GetFlights()).Returns(FlightViewAsyncGenerator(returnFlightViews));

            FlightController controller = new FlightController(service.Object);

            ObjectResult? response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

            Queue<FlightView>? content = response.Value as Queue<FlightView>;
            Assert.IsNotNull(content);

            Assert.IsTrue(returnFlightViews.All(flight => content.Contains(flight)));

        }

        [TestMethod]
        public async Task GetFlights_failure_FlightNotFound_404()
        {
            Mock<FlightService> service = new();
            service.Setup(s => s.GetFlights()).Throws(new FlightNotFoundException());
            FlightController controller = new FlightController(service.Object);
            ObjectResult? response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("No flights were found in the database", response.Value);
        }

        [TestMethod]
        public async Task GetFlights_failure_InternalError_500()
        {
            Mock<FlightService> service = new();
            service.Setup(s => s.GetFlights()).Throws(new ArgumentException());
            FlightController controller = new FlightController(service.Object);
            ObjectResult? response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("An error occurred", response.Value);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_success()
        {
            Mock<FlightService> service = new();
            service.Setup(s => s.GetFlightByFlightNumber(1)).ReturnsAsync(new FlightView("1", ("Tehran", "10"), ("Arlanda", "20")));
            FlightController controller = new FlightController(service.Object);
            ObjectResult? result = await controller.GetFlightByFlightNumber(1) as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            FlightView? flight = result.Value as FlightView;
            Assert.IsNotNull(flight);
            Assert.AreEqual("1", flight.FlightNumber);
            Assert.AreEqual("Tehran", flight.Origin.City);
            Assert.AreEqual("10", flight.Origin.Code);
            Assert.AreEqual("Arlanda", flight.Destination.City);
            Assert.AreEqual("20", flight.Destination.Code);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_failure_FlightNotFound_404()
        {
            Mock<FlightService> service = new();
            service.Setup(s => s.GetFlightByFlightNumber(1)).Throws(new FlightNotFoundException());
            FlightController controller = new FlightController(service.Object);
            ObjectResult? result = await controller.GetFlightByFlightNumber(1) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.AreEqual("No flights were found in the database", result.Value);
        }
        [TestMethod]
        public async Task GetFlightByFlightNumber_failure_BadRequest_400()
        {
            Mock<FlightService> service = new();
            service.Setup(s => s.GetFlightByFlightNumber(1)).Throws(new ArgumentException());
            FlightController controller = new FlightController(service.Object);
            ObjectResult? result = await controller.GetFlightByFlightNumber(1) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Bad Request", result.Value);
        }

    }
}
