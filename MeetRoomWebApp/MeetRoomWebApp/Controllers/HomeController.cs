using MeetRoomWebApp.Models;
using MeetRoomWebApp.Models.BindingModels;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            ViewBag.Users = _userStorage.GetFullList().Where(rec => rec.Email != User.Identity.Name);
            ViewBag.Time = time;

            return View(UpdateData());
        }

        private Dictionary<DateTime, List<SessionViewModel>> UpdateData()
        {
            int diff = (7 + (Program.DayInWeek.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = Program.DayInWeek.AddDays(-1 * diff).Date;

            List<DateTime> daysInWeek = Enumerable.Range(0, 7).Select(d => startOfWeek.AddDays(d)).ToList();

            Dictionary<DateTime, List<SessionViewModel>> resultSessions = new Dictionary<DateTime, List<SessionViewModel>>();

            foreach(var date in daysInWeek)
            {
                var sessionsOfDate = _sessionStorage.GetFilteredList(new SessionBindingModel { DateSession = date })
                    .OrderBy(rec => rec.DateSession)
                    .ToList();

                resultSessions.Add(date, sessionsOfDate);
            }

            return resultSessions;
        }

        [HttpPost]
        public IActionResult Index(DateTime bookingDate, string bookingTime, int duration, List<string> users)
        {
            string bookingDateTimeStr = $"{bookingDate.Date.ToShortDateString().Replace(".", "-")} {bookingTime}:00";
            DateTime bookingDateTime = DateTime.Parse(bookingDateTimeStr);

            if (bookingDateTime <= DateTime.Now)
            {
                throw new Exception("Вы указали дату или время меньше текущего");
            }
            if(duration < 30 && duration % 30 != 0)
            {
                throw new Exception("Укажите корректную длительность сеанса");
            }

            Dictionary<string, string> usersDict = new Dictionary<string, string>();

            var creator = _userStorage.GetElement(User.Identity.Name);

            usersDict.Add(creator.Id, creator.Email);
            users.ForEach(userId => usersDict.Add(userId, _userStorage.GetElement(userId).Email));

            _sessionStorage.Insert(new SessionBindingModel 
            {
                DateSession = bookingDateTime, 
                SessionDuration = duration, 
                UserSessions = usersDict
            });

            return Redirect("/Home/Index");
        }

        [HttpPost]
        public void IncrementWeekForward()
        {
            Program.DayInWeek = Program.DayInWeek.AddDays(7);
        }

        [HttpPost]
        public void IncrementWeekBack()
        {
            Program.DayInWeek = Program.DayInWeek.AddDays(-7);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
