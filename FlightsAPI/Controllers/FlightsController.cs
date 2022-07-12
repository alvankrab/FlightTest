using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightsAPI.Controllers
{
    [Route("Flights")]
    [ApiController]
    public class FlightsController : ControllerBase
    {


        [HttpGet("GetFlights")]
        public async Task<IEnumerable<FlightsModel>> GetFlights()
        {
                var firebaseClient = new FirebaseClient("https://airline-test-proj-default-rtdb.firebaseio.com/");
                var result = await firebaseClient
                    .Child("Flights")
                    .OnceAsync<FlightsModel>();

                var flights = new List<FlightsModel>();
                foreach (var item in result)
                {
                    flights.Add(item.Object);
                }
            return flights;
        }

        [HttpPost("AddFlight")]
        public async Task<HttpResponseMessage> AddFlight([FromBody] FlightsModel flight)
        {
            var response = new HttpResponseMessage();
            try
            {
                var firebaseClient = new FirebaseClient("https://airline-test-proj-default-rtdb.firebaseio.com/");
                var result = await firebaseClient
                    .Child("Flights")
                    .PostAsync(flight);
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
