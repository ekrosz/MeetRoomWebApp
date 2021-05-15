using MeetRoomWebApp.Models;
using MeetRoomWebApp.Models.BindingModels;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MeetRoomWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ISessionStorage _sessionStorage;

        private readonly IUserStorage _userStorage;

        public HomeController(ILogger<HomeController> logger, ISessionStorage sessionStorage, IUserStorage userStorage)
        {
            _logger = logger;
            _sessionStorage = sessionStorage;
            _userStorage = userStorage;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            string ID = Activity.Current?.Id;
            List<DateTime> days = new List<DateTime>();
            DayOfWeek Day = DateTime.Now.DayOfWeek;
            int Days = Day - DayOfWeek.Monday;
            for (int i = 0; i < 7; i++)
            {
                days.Add(DateTime.Now.AddDays(-Days).AddDays(i));
            }

            Dictionary<DateTime, List<SessionViewModel>> resultSessions = new Dictionary<DateTime, List<SessionViewModel>>();

            foreach(var date in days)
            {
                var sessionsOfDate = _sessionStorage.GetFilteredList(new SessionBindingModel { DateSession = date })
                    .OrderBy(rec => rec.DateSession)
                    .ToList();

                resultSessions.Add(date, sessionsOfDate);
            }

            List<string> time = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                {
                    time.Add($"0{ i }:00");
                    time.Add($"0{ i }:30");
                }
                else
                {
                    time.Add($"{ i }:00");
                    time.Add($"{ i }:30");
                }
            }

            ViewBag.Users = _userStorage.GetFullList().Where(rec => rec.Id != ID);
            ViewBag.Time = time;

            return View(resultSessions);
        }

        [HttpPost]
        public void Index(DateTime bookingDate, string bookingTime, int duration, Dictionary<string, string> user)
        {
            string bookingDateTimeStr = $"{bookingDate.Date} {bookingTime}:00";
            DateTime bookingDateTime = DateTime.Parse(bookingDateTimeStr);

            if (bookingDateTime <= DateTime.Now)
            {
                throw new Exception("Вы указали дату или время меньше текущего");
            }
            if (duration < 30 && duration % 30 != 0)
            {
                throw new Exception("Укажите корректную длительность сеанса");
            }


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
