using FlyingDutchmanAirlines.Controllers.JsonData;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;

namespace FlyingDutchmanAirlines.Controllers
{
    class BookingModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentException();
            }

            ReadResult result = await
            bindingContext.HttpContext.Request.BodyReader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            string bodyJson = Encoding.UTF8.GetString(buffer.FirstSpan);
            JObject bodyJsonObject = JObject.Parse(bodyJson);

            BookingData boundData = new BookingData
            {
                FirstName = (string)bodyJsonObject["FirstName"],
                LastName = (string)bodyJsonObject["LastName"]
            };

            bindingContext.Result = ModelBindingResult.Success(boundData);
        }
    }
}
