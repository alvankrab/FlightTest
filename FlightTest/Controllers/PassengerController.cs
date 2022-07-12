using FlightTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightTest.Controllers
{
    public class PassengerController : Controller
    {
        // GET: PassengerController
        public async Task<ActionResult> Index(int flightnumber = 0)
        {
            ViewBag.flightnumber = flightnumber;
            if (flightnumber == 0)
            {
                ViewBag.PassengersList = null;
                ViewBag.viewPassengers = false;
            }
            else
            {

                ViewBag.viewPassengers = true;
                string apiUrl = "https://localhost:7184/Passenger/GetPassengers?flightNumber=" + flightnumber.ToString();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var passengers = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Passenger>>(data);
                        ViewBag.PassengersList = passengers;
                        return View();
                    }

                }
            }
            return View();
        }


        // POST: PassengerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7184/");
                    var postTask = client.PostAsJsonAsync<Passenger>("Passenger/AddPassenger", passenger);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { flightnumber = passenger.FlightNumber } );
                    }
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return RedirectToAction("Index");
        }

    }
}
