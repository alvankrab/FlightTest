using FlightTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<FlightsModel> flightsList;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string apiUrl = "https://localhost:7184/Flights/GetFlights";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var flights = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<FlightsModel>>(data);
                    flightsList = new List<FlightsModel>();
                    flightsList.AddRange(flights.ToList());
                    ViewBag.Flights = flightsList;
                }


            }

            return View();
        }


        [HttpPost]
        public ActionResult Flights(FlightsModel flight)
        {
            var id = flight.FlightNumber;
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7184/");
                    var postTask = client.PostAsJsonAsync<FlightsModel>("Flights/AddFlight", flight);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Passenger(int PassengerId)
        {
            return RedirectToAction("Index", "Passenger", new { PassengerId });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}