using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test.RepositoryLayer
{
    [TestClass]
    public class CusomterRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private CustomerRepository _repository;
        [TestInitialize]
        public async Task TesInitialize()
        {
            _context = TestTools.InitializeInMemoryContext("FlyingDutchman");

            Customer testCustomer = new("Linus Torvalds");
            _context.Customers.Add(testCustomer);
            await _context.SaveChangesAsync();


            _repository = new CustomerRepository(_context);
            Assert.IsNotNull(_repository);
        }
        [TestMethod]
        public async Task CreateCustomer_Success()
        {
            bool result = await _repository.CreateCustomer("Jack Daniel");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotAddCustomerToDatabase))]
        public async Task CreateCustomer_failure_DatabaseError()
        {
            _ = await _repository.CreateCustomer("Error Name");

        }
        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsNull()
        {
            bool result = await _repository.CreateCustomer(null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsEmptyString()
        {
            bool result = await _repository.CreateCustomer("");
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow('!')]
        [DataRow('#')]
        [DataRow('$')]
        [DataRow('%')]
        [DataRow('&')]
        [DataRow('*')]
        public async Task CreateCustomer_Failure_NameContainsInvalidChars(char invalidCharacter)
        {
            bool result = await _repository.CreateCustomer("Donald Knuth " + invalidCharacter);
            Assert.IsFalse(result);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("#")]
        [DataRow("$")]
        [DataRow("%")]
        [DataRow("&")]
        [DataRow("*")]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public async Task GetCustomerByName_Failure_InvalidName(string name)
        {
            await _repository.GetCustomerByName(name);
        }

        [TestMethod]
        public async Task GetCustomerByName_Success()
        {
            Customer customer = await _repository.GetCustomerByName("Linus Torvalds");
            Assert.IsNotNull(customer);

            Customer dbCustomer = await _context.Customers.FirstAsync();

            Assert.AreEqual(customer, dbCustomer);
        }

    }
}