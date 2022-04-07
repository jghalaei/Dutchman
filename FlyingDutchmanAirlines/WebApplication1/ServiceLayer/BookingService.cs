using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using System.Runtime.ExceptionServices;

namespace FlyingDutchmanAirlines.ServiceLayer
{
    public class BookingService
    {
        private BookingRepository _bookingRepository;
        private CustomerRepository _customerRepository;
        private FlightRepository _flightRepository;
        public BookingService(BookingRepository bookingRepository, FlightRepository flightRepository, CustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _flightRepository = flightRepository;
        }

        private async Task<bool> isFlightExistInDatabase(int flightNumber)
        {
            try
            {
                return await _flightRepository.GetFlightByFlightNumber(flightNumber) != null;
            }
            catch (FlightNotFoundException)
            {
                return false;
            }
        }
        private async Task<Customer?> GetCusotmerFromDatabase(string name)
        {
            try
            {
                return await _customerRepository.GetCustomerByName(name);

            }
            catch (CustomerNotFoundException)
            {
                return null;
            }
            catch (Exception exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException
                    ?? new Exception()).Throw();
                return null;
            }
        }
        private async Task<Customer> AddCustomerToDatabase(string name)
        {
            await _customerRepository.CreateCustomer(name);
            return await _customerRepository.GetCustomerByName(name);
        }
        public async Task<(bool result, Exception? exception)> CreateBooking(string name, int flightNumber)
        {
            if (String.IsNullOrEmpty(name))
            {
                return (false, new ArgumentException());
            }
            Customer customer;
            try
            {
                customer = await GetCusotmerFromDatabase(name)
                        ?? await AddCustomerToDatabase(name);
                if (!await isFlightExistInDatabase(flightNumber))
                    throw new CouldNotAddBookingToDatabaseException();
                await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);
                return (true, null);
            }
            catch (Exception exception)
            {
                return (false, exception);
            }

        }
    }
}
