using MeetRoomWebApp.Models.BindingModels;
using MeetRoomWebApp.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace MeetRoomWebApp.Controllers
{
    /// <summary>
    /// Controller responsible for the logic of the "Session" object.
    /// Views: Index, Create, Edit, Details, Delete.
    /// </summary>
    public class SessionController : Controller
    {
        private readonly ISessionStorage _sessionStorage;

        private readonly IUserStorage _userStorage;

        public SessionController(ISessionStorage sessionStorage, IUserStorage userStorage)
        {
            _sessionStorage = sessionStorage;
            _userStorage = userStorage;
        }

        // GET: Session
        [Authorize]
        public IActionResult Index()
        {
            return View(_sessionStorage.GetFilteredListByUser(User.Identity.Name));
        }

        // GET: Session/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = _sessionStorage.GetElement(new SessionBindingModel
            {
                Id = id
            });

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Session/Create
        public IActionResult Create()
        {
            ViewData["GuestsId"] = new MultiSelectList(
                _userStorage.GetFullList()
                    .Where(rec => rec.Email != User.Identity.Name)
                    .ToList(),
                "Id",
                "Email");

            return View();
        }

        // POST: Session/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DateSession,SessionDuration,Guests")] SessionBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var creator = _userStorage.GetElement(User.Identity.Name);

                model.Guests.Add(creator.Id);
                _sessionStorage.Insert(model);

                return Redirect("/Home/Index");
            }

            ViewData["GuestsId"] = new MultiSelectList(
                _userStorage.GetFullList()
                    .Where(rec => rec.Email != User.Identity.Name)
                    .ToList(), 
                "Id",
                "Email");

            return View(model);
        }

        // GET: Session/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = _sessionStorage.GetElement(new SessionBindingModel
            {
                Id = id
            });

            if (session == null)
            {
                return NotFound();
            }

            ViewData["GuestsId"] = new MultiSelectList(
                _userStorage.GetFullList()
                    .Where(rec => rec.Email != User.Identity.Name)
                    .ToList(),
                "Id",
                "Email");

            return View(new SessionBindingModel 
            { 
                Id = session.Id,
                DateSession = session.DateSession,
                SessionDuration = session.SessionDuration,
                Guests = session.Guests
                .Select(rec => rec.Id)
                .ToList()
            });
        }

        // POST: Session/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,DateSession,SessionDuration,Guests")] SessionBindingModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var creator = _userStorage.GetElement(User.Identity.Name);

                    model.Guests.Add(creator.Id);

                    _sessionStorage.Update(model);
                }
                catch (Exception)
                {
                    if (!SessionExists(model.Id.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Session/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = _sessionStorage.GetElement(new SessionBindingModel
            {
                Id = id
            });

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Session/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _sessionStorage.Delete(new SessionBindingModel
            {
                Id = id
            });

            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _sessionStorage.GetFullList().Any(e => e.Id == id);
        }
    }
}
