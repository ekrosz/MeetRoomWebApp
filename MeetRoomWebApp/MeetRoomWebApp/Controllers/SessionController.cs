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
        /// <summary>
        /// Session interface.
        /// </summary>
        private readonly ISessionStorage _sessionStorage;

        /// <summary>
        /// User interface.
        /// </summary>
        private readonly IUserStorage _userStorage;

        public SessionController(ISessionStorage sessionStorage, IUserStorage userStorage)
        {
            _sessionStorage = sessionStorage;
            _userStorage = userStorage;
        }

        /// <summary>
        /// Loading the list of sessions
        /// </summary>
        /// <returns>Session list model</returns>
        [Authorize]
        public IActionResult Index()
        {
            return View(_sessionStorage.GetFilteredListByUser(User.Identity.Name));
        }

        /// <summary>
        /// Loading data for a specific session
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Specific session data (session model)</returns>
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

        /// <summary>
        /// Loading data for the creation page (get-method)
        /// </summary>
        /// <returns>View</returns>
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

        /// <summary>
        /// Session creation method (post-method)
        /// </summary>
        /// <param name="model">New session model</param>
        /// <returns>Redirects to index page or leaves on create page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DateSession,SessionDuration,Guests")] SessionBindingModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var creator = _userStorage.GetElement(User.Identity.Name);

                    model.Guests.Add(creator.Id);
                    _sessionStorage.Insert(model);

                    return Redirect("/Home/Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            ViewData["GuestsId"] = new MultiSelectList(
                _userStorage.GetFullList()
                    .Where(rec => rec.Email != User.Identity.Name)
                    .ToList(), 
                "Id",
                "Email");

            return View(model);
        }

        /// <summary>
        /// Loading data for a specific session (get-method)
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Specific session data (session model)</returns>
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

        /// <summary>
        /// Method for modifying data in the session (post-method)
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <param name="model">Session model</param>
        /// <returns>Redirects to index page or leaves on create page</returns>
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

                    return Redirect("/Session/Index");
                }
                catch (Exception ex)
                {
                    if (!SessionExists(model.Id.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: " + ex.Message);
                    }
                }
            }

            ViewData["GuestsId"] = new MultiSelectList(
                _userStorage.GetFullList()
                    .Where(rec => rec.Email != User.Identity.Name)
                    .ToList(),
                "Id",
                "Email");

            return View(model);
        }

        /// <summary>
        /// Loading data for a specific session (get-method)
        /// </summary>
        /// <param name="id">session ID</param>
        /// <returns>Specific session data (session model)</returns>
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

        /// <summary>
        /// Session deletion method by ID (post-method)
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Redirects to index page</returns>
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

        /// <summary>
        /// Checking for Session Existence by ID
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>True or false</returns>
        private bool SessionExists(int id)
        {
            return _sessionStorage.GetFullList().Any(e => e.Id == id);
        }
    }
}
