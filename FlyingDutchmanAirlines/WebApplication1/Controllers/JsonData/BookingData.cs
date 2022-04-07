using System.ComponentModel.DataAnnotations;

namespace FlyingDutchmanAirlines.Controllers.JsonData
{
    public class BookingData:IValidatableObject
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => _firstName = ValidateName(value, nameof(FirstName));
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => _lastName = ValidateName(value, nameof(LastName));
        }


        private string ValidateName(string name, string propertyName)
        {
            return string.IsNullOrEmpty(name)
    ? throw new InvalidOperationException("could not set " + propertyName) : name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new();
            if (FirstName == null && LastName == null)
            {
                results.Add(new ValidationResult("all given data points are null"));
            }
            if (FirstName == null || LastName == null)
            {
                results.Add(new ValidationResult("One of the given datapoints are null"));
            }
            return results;
        }
    }
}