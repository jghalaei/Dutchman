using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlyingDutchmanAirlines.Controllers
{
    [Route("Flight")]
    public class FlightController :Controller
    {
        private readonly FlightService _service;
        public FlightController(FlightService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            try{
                Queue<FlightView> flightViews = new Queue<FlightView>();
                await foreach (FlightView flight in _service.GetFlights())
                {
                    flightViews.Enqueue(flight);
                }
                return StatusCode((int)HttpStatusCode.OK, flightViews);
            }
            catch (FlightNotFoundException)
            {
                return StatusCode((int)  HttpStatusCode.NotFound, "No flights were found in the database");
            }
            catch(Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        [HttpGet("{flightNumber}")]
        public async Task<IActionResult> GetFlightByFlightNumber(int flightNumber)
        {
            try
            {
                if (!flightNumber.IsPositive())
                {
                    throw new Exception();
                }
                FlightView view = await _service.GetFlightByFlightNumber(flightNumber);
                return StatusCode((int) HttpStatusCode.OK,view);
            }
            catch (FlightNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound, "No flights were found in the database");
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Bad Request");
            }
        }
    }
}
