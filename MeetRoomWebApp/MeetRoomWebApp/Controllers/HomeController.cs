using MeetRoomWebApp.Models;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeetRoomWebApp.Controllers
{
    /// <summary>
    /// The default controller responsible for the main page (view "Index").
    /// Index contains: table of bookings.
    /// </summary>
    public class HomeController : Controller
    {
        public readonly string[] time;

        private readonly ISessionStorage _sessionStorage;

        public HomeController(ISessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;

             /* 
             * Magic number 49: so that the end of the axis is 00:00
             * Magic number 30: it is 30 minutes (min axis step)
             */
            time = Enumerable.Range(0, 49)
                .Select(i => TimeSpan.FromMinutes(i * 30).ToString(@"hh\:mm"))
                .ToArray();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string currentWeek)
        {
            if (currentWeek == null)
            {
                // This week
                currentWeek = $"{DateTime.Now.Year}-W{Math.Round((double)(DateTime.Now.DayOfYear / 7))}";
            }

            return View((UpdateData(currentWeek), time, currentWeek));
        }

        private Dictionary<DateTime, SessionViewModel[]> UpdateData(string currentWeek)
        {
            Dictionary<DateTime, SessionViewModel[]> resultSessions = new Dictionary<DateTime, SessionViewModel[]>();

            int year = Convert.ToInt32(currentWeek.Split('-')[0]);
            int week = Convert.ToInt32(currentWeek.Split('W')[1]);

            DateTime startOfWeek = new DateTime();

            /*
             * Magic number 1: DateTime startOfWeek = new DateTime() => startOfWeek = "01/01/0001"
             * Magic number 3: difference of days between Thursday and Monday, because ("01/01/YYYY").AddDays(7 * week) is always Thursday
             * Magic number 7: number of days in a week
             */

            startOfWeek = startOfWeek.AddYears(year - 1).AddDays((7 * week) - 3 - 1);

            var sessionsOfWeek = _sessionStorage.GetFilteredListByWeek(startOfWeek, startOfWeek.AddDays(6))
                    .OrderBy(rec => rec.DateSession)
                    .ToList();

            var daysInWeek = Enumerable.Range(0, 7)
                .Select(day => startOfWeek.AddDays(day))
                .ToArray();

            foreach(var date in daysInWeek)
            {
                var sessionsOfDate = sessionsOfWeek
                    .Where(rec => rec.DateSession.Date == date.Date)
                    .OrderBy(rec => rec.DateSession)
                    .ToArray();

                resultSessions.Add(date, sessionsOfDate);
            }

            return resultSessions;
        }

        public IActionResult GetWeek(string week)
        {
            return Redirect($"/Home/Index?currentWeek={week}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
