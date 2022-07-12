using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightsAPI.Controllers
{
    [Route("Passenger")]
    [ApiController]
    public class PassengerController : ControllerBase
    {

        // GET: api/<FlightsController>
        [HttpGet("GetPassengers")]
        public async Task<IEnumerable<Passenger>> GetPassengers(int flightNumber)
        {
            var firebaseClient = new FirebaseClient("https://airline-test-proj-default-rtdb.firebaseio.com/");
            var result = await firebaseClient
                .Child("Passengers")
                .Child(flightNumber.ToString())
                .OnceAsync<Passenger>();

            var passenger = new List<Passenger>();
            foreach (var item in result)
            {
                passenger.Add(item.Object);
            }
            return passenger;
        }

        [HttpPost("AddPassenger")]
        public async Task<HttpResponseMessage> AddPassenger([FromBody] Passenger passenger)
        {
            var response = new HttpResponseMessage();
            try
            {
                var firebaseClient = new FirebaseClient("https://airline-test-proj-default-rtdb.firebaseio.com/");
                var result = await firebaseClient
                .Child("Passengers")
                .Child(passenger.FlightNumber.ToString())
                .PostAsync(passenger);
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
    }
}
