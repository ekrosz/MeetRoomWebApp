using System;
using System.Collections.Generic;
using System.Linq;
using MeetRoomWebApp.Data;
using MeetRoomWebApp.Models.BindingModels;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using MeetRoomWebApp.Data.Entities;

namespace MeetRoomWebApp.Models.Implements
{
    /// <summary>
    /// ISessionStorage interface implementation class
    /// </summary>
    public class SessionStorage : ISessionStorage
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly MeetRoomDbContext _context;

        public SessionStorage(MeetRoomDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Full list of sessions
        /// </summary>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFullList()
        {
            return _context.Sessions
                .Include(rec => rec.UserSessions)
                .ThenInclude(rec => rec.User)
                .Select(rec => new SessionViewModel
                {
                    Id = rec.Id,
                    DateSession = rec.DateSession,
                    SessionDuration = rec.SessionDuration,
                    Guests = rec.UserSessions
                    .Where(userSession => userSession.SessionId == rec.Id)
                    .Select(userSession => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Id,
                        Email = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Email
                    })
                    .ToList()
                })
                .ToList();
        }

        /// <summary>
        /// List of sessions for the specified week
        /// </summary>
        /// <param name="dateFrom">Date of the first day of the week</param>
        /// <param name="dateTo">Date of the last day of the week</param>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFilteredListByWeek(DateTime dateFrom, DateTime dateTo)
        {
            var intermediateResult = _context.Sessions
                .Include(rec => rec.UserSessions)
                .ThenInclude(rec => rec.User)
                .Where(rec => rec.DateSession.Date >= dateFrom && rec.DateSession.Date <= dateTo)
                .Select(rec => new SessionViewModel
                {
                    Id = rec.Id,
                    DateSession = rec.DateSession,
                    SessionDuration = rec.SessionDuration,
                    Guests = rec.UserSessions
                    .Where(userSession => userSession.SessionId == rec.Id)
                    .Select(userSession => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Id,
                        Email = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Email
                    })
                    .ToList()
                })
                .ToList();

            List<SessionViewModel> result = new List<SessionViewModel>();

            foreach (var session in intermediateResult)
            {
                if (CheckDoubleModel(session))
                {
                    // Additional model
                    SessionViewModel additionalModel = new SessionViewModel
                    {
                        Id = session.Id,
                        DateSession = session.DateSession.AddDays(1).Date,
                        SessionDuration = session.SessionDuration - (session.DateSession.AddDays(1).Date - session.DateSession),
                        Guests = session.Guests
                    };

                    // Main model
                    session.SessionDuration = session.DateSession.AddDays(1).Date - session.DateSession;

                    result.Add(session);
                    result.Add(additionalModel);

                    continue;
                }

                result.Add(session);
            }

            return result;
        }

        /// <summary>
        /// List of sessions the specified user is in
        /// </summary>
        /// <param name="user">User's email</param>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFilteredListByUser(string user)
        {
            var sessionIds = _context.UserSessions
                .Include(rec => rec.User)
                .Where(rec => rec.User.UserName == user)
                .Select(rec => rec.SessionId)
                .ToList();

            return _context.Sessions
                .Include(rec => rec.UserSessions)
                .ThenInclude(rec => rec.User)
                .Where(rec => sessionIds.Contains(rec.Id))
                .ToList()
                .Select(rec => new SessionViewModel
                {
                    Id = rec.Id,
                    DateSession = rec.DateSession,
                    SessionDuration = rec.SessionDuration,
                    Guests = rec.UserSessions
                    .Where(userSession => userSession.SessionId == rec.Id)
                    .Select(userSession => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Id,
                        Email = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Email
                    })
                    .ToList()
                })
                .OrderBy(rec => rec.DateSession)
                .ToList();
        }

        /// <summary>
        /// Search for a session by ID
        /// </summary>
        /// <param name="model">Model with ID</param>
        /// <returns>Session</returns>
        public SessionViewModel GetElement(SessionBindingModel model)
        {
            var session = _context.Sessions
                    .Include(rec => rec.UserSessions)
                    .ThenInclude(rec => rec.User)
                    .FirstOrDefault(rec => rec.Id == model.Id);

            return session != null ?
                new SessionViewModel
                {
                    Id = session.Id,
                    DateSession = session.DateSession,
                    SessionDuration = session.SessionDuration,
                    Guests = session.UserSessions
                    .Where(userSession => userSession.SessionId == session.Id)
                    .Select(userSession => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Id,
                        Email = _context.Users.FirstOrDefault(user => user.Id == userSession.UserId).Email
                    })
                    .ToList()
                } :
                null;
        }

        /// <summary>
        /// Adding a new session to the database
        /// </summary>
        /// <param name="model">New session model</param>
        public void Insert(SessionBindingModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var session = new Session
                    {
                        DateSession = model.DateSession,
                        SessionDuration = model.SessionDuration
                    };

                    _context.Sessions.Add(session);
                    _context.SaveChanges();

                    foreach (var userId in model.Guests)
                    {
                        _context.UserSessions.Add(new UserSession
                        {
                            SessionId = session.Id,
                            UserId = userId
                        });
                    }
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }

        /// <summary>
        /// Updating an existing session in the database
        /// </summary>
        /// <param name="model">New session model</param>
        public void Update(SessionBindingModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!IntersectionСheck(model))
                    {
                        throw new Exception("there is already a booking during this period");
                    }

                    // Updating DateSession and SessionDuration
                    var session = _context.Sessions
                        .FirstOrDefault(rec => rec.Id == model.Id);

                    session.DateSession = model.DateSession;
                    session.SessionDuration = model.SessionDuration;

                    _context.Sessions.Update(session);
                    _context.SaveChanges();

                    // Updating guests list
                    var sessionUsers = _context.UserSessions
                        .Where(rec => rec.SessionId == model.Id)
                        .ToList();

                    _context.UserSessions
                        .RemoveRange(sessionUsers);

                    _context.SaveChanges();

                    foreach (var item in model.Guests)
                    {
                        _context.UserSessions.Add(new UserSession
                        {
                            SessionId = model.Id.Value,
                            UserId = item
                        });
                    }
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }

        /// <summary>
        /// Removing the specified session from the database by identifier
        /// </summary>
        /// <param name="model">Session model with identifier</param>
        public void Delete(SessionBindingModel model)
        {
            var element = _context.Sessions
                .FirstOrDefault(rec => rec.Id == model.Id);

            if (element != null)
            {
                _context.Sessions.Remove(element);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("booking not found!");
            }
        }

        /// <summary>
        /// Checking the intersection of the specified session with other sessions
        /// </summary>
        /// <param name="model">Specified session</param>
        /// <returns>True or false</returns>
        private bool IntersectionСheck(SessionBindingModel model)
        {
                var otherSessions = _context.Sessions
                    .Where(rec => (rec.DateSession.Date == model.DateSession.Date || rec.DateSession.Date == model.DateSession.AddDays(1).Date) && rec.Id != model.Id)
                    .OrderBy(rec => rec.DateSession)
                    .ToList();

            foreach (var item in otherSessions)
            {
                if (model.DateSession < item.DateSession && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) > item.DateSession ||
                    model.DateSession < item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes) && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) > item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes) ||
                    model.DateSession > item.DateSession && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) < item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checking session duration.
        /// If the session starts and ends on the same day, true is returned.
        /// </summary>
        /// <param name="model">Specified session</param>
        /// <returns>True or false</returns>
        private bool CheckDoubleModel(SessionViewModel model)
        {
            if ((model.DateSession.Date == model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Date) ||
                (model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Hour == 0 && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Minute == 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
