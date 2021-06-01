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
        /// <summary>
        /// Array of time. Vertical axis on the home page.
        /// </summary>
        private readonly string[] time;

        /// <summary>
        /// Session interface.
        /// </summary>
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

        /// <summary>
        /// Index page method (get method)
        /// </summary>
        /// <param name="selectedWeek">Specified week to load a specific list of sessions</param>
        /// <returns>Tuple of objects (sessions, time axis, selected week)</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Index(string selectedWeek)
        {
            if (selectedWeek == null)
            {
                // This week
                selectedWeek = $"{DateTime.Now.Year}-W{Math.Round((double)DateTime.Now.DayOfYear / 7)}";
            }

            return View((UpdateData(selectedWeek), time, selectedWeek));
        }

        /// <summary>
        /// Method for updating data (sessions) on the home page
        /// </summary>
        /// <param name="selectedWeek">Specified week to load a specific list of sessions</param>
        /// <returns>Dictionary<Days, Session arrays></returns>
        private Dictionary<DateTime, SessionViewModel[]> UpdateData(string selectedWeek)
        {
            Dictionary<DateTime, SessionViewModel[]> resultSessions = new Dictionary<DateTime, SessionViewModel[]>();

            int year = Convert.ToInt32(selectedWeek.Split('-')[0]);
            int week = Convert.ToInt32(selectedWeek.Split('W')[1]);

            DateTime startOfWeek = new DateTime();
             
            // In the DayOfWeek enum, Sunday = 0
            if (startOfWeek.AddYears(year - 1).DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = startOfWeek.AddYears(year - 1).AddDays((7 * week) - 6);
            }
            else
            {
                startOfWeek = startOfWeek.AddYears(year - 1).AddDays((7 * week) - (int)startOfWeek.AddYears(year - 1).DayOfWeek + 1);
            }

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

        /// <summary>
        /// Method invoked by the user when changing the week on the main page for updating data
        /// </summary>
        /// <param name="week">Specified week</param>
        /// <returns>Updated data</returns>
        public IActionResult GetWeek(string week)
        {
            return Redirect($"/Home/Index?currentWeek={week}");
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <returns>Error view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
