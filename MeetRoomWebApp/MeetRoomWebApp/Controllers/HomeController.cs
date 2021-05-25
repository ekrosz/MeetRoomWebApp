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

namespace MeetRoomWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionStorage _sessionStorage;

        private readonly IUserStorage _userStorage;

        public HomeController(ISessionStorage sessionStorage, IUserStorage userStorage)
        {
            _sessionStorage = sessionStorage;
            _userStorage = userStorage;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            DateTime dateTimeTemp = DateTime.Parse("00:00:00");

            List<string> time = Enumerable.Range(0, 48)
                .Select(i => dateTimeTemp.AddMinutes(i * 30).ToString("HH:mm"))
                .ToList();

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
        public IActionResult Index(DateTime bookingDateTime, DateTime duration, List<string> users)
        {
            if (bookingDateTime <= DateTime.Now)
            {
                throw new Exception("Вы указали дату или время меньше текущего");
            }
            if (bookingDateTime.Minute % 30 != 0)
            {
                throw new Exception("Укажите корректное время начала сеанса");
            }
            if (duration.Minute + duration.Hour * 60 < 30 || duration.Minute % 30 != 0)
            {
                throw new Exception("Укажите корректную длительность сеанса");
            }

            Dictionary<string, string> usersDict = new Dictionary<string, string>();

            var creator = _userStorage.GetElement(User.Identity.Name);

            if (creator == null)
            {
                throw new Exception("Создатель сеанса не найден");
            }

            usersDict.Add(creator.Id, creator.Email);

            foreach(var userId in users)
            {
                var userEmail = _userStorage.GetElement(userId).Email;

                if(userEmail == null)
                {
                    throw new Exception("Добавленный гость не найден");
                }

                usersDict.Add(userId, userEmail);
            }

            _sessionStorage.Insert(new SessionBindingModel 
            {
                DateSession = bookingDateTime, 
                SessionDuration = (duration.Hour * 60) + duration.Minute, 
                UserSessions = usersDict
            });

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult GetNextWeek()
        {
            Program.DayInWeek = Program.DayInWeek.AddDays(7);

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult GetLastWeek()
        {
            Program.DayInWeek = Program.DayInWeek.AddDays(-7);

            return Redirect("/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
