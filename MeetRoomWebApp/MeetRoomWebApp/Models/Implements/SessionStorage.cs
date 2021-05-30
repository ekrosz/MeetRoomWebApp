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
        private readonly MeetRoomDbContext _context;

        public SessionStorage(MeetRoomDbContext context)
        {
            _context = context;
        }

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
                    .Select(user => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Id,
                        Email = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Email
                    })
                    .ToList()
                })
                .ToList();
        }

        public List<SessionViewModel> GetFilteredListByWeek(DateTime dateFrom, DateTime dateTo)
        {
            return _context.Sessions
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
                    .Select(user => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Id,
                        Email = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Email
                    })
                    .ToList()
                })
                .ToList();
        }

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
                    .Select(user => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Id,
                        Email = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Email
                    })
                    .ToList()
                })
                .ToList();
        }

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
                    .Select(user => new UserViewModel
                    {
                        Id = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Id,
                        Email = _context.Users.FirstOrDefault(x => x.Id == user.UserId).Email
                    })
                    .ToList()
                } :
                null;
        }

        public void Insert(SessionBindingModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if ((model.DateSession.Date == model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Date) ||
                        (model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Hour == 0 && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes).Minute == 0))
                    {
                        CreateModel(model, new Session(), _context);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var firstModel = new SessionBindingModel
                        {
                            DateSession = model.DateSession,
                            SessionDuration = model.DateSession.AddDays(1).Date - model.DateSession,
                            Guests = model.Guests
                        };
                        var secondModel = new SessionBindingModel
                        {
                            DateSession = model.DateSession.AddDays(1).Date,
                            SessionDuration = model.SessionDuration - firstModel.SessionDuration,
                            Guests = model.Guests
                        };

                        CreateModel(firstModel, new Session(), _context);
                        CreateModel(secondModel, new Session(), _context);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }

        public void Update(SessionBindingModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var sessionUsers = _context.UserSessions
                        .Where(rec => rec.SessionId == model.Id)
                        .ToList();

                    foreach (var item in sessionUsers)
                    {
                        _context.UserSessions.Remove(item);
                    }
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
                throw new Exception("Элемент не найден");
            }
        }

        private Session CreateModel(SessionBindingModel model, Session session, MeetRoomDbContext context)
        {
            var otherSessions = context.Sessions
                .Where(rec => rec.DateSession.Date == model.DateSession.Date || rec.DateSession.Date == model.DateSession.AddDays(1).Date)
                .OrderBy(rec => rec.DateSession);

            foreach(var item in otherSessions)
            {
                if(model.DateSession < item.DateSession && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) > item.DateSession ||
                    model.DateSession < item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes) && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) > item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes) ||
                    model.DateSession > item.DateSession && model.DateSession.AddMinutes(model.SessionDuration.TotalMinutes) < item.DateSession.AddMinutes(item.SessionDuration.TotalMinutes))
                {
                    throw new Exception("В этом промежутке времени есть бронь");
                }
            }

            session.DateSession = model.DateSession;
            session.SessionDuration = model.SessionDuration;

            if (session.Id == 0)
            {
                    context.Sessions.Add(session);
                    context.SaveChanges();
            }

            if (model.Id.HasValue)
            {
                var sessionUsers = context.UserSessions
                    .Where(rec => rec.SessionId == model.Id.Value)
                    .ToList();

                context.UserSessions
                    .RemoveRange(sessionUsers
                        .Where(rec => !model.Guests
                            .Contains(rec.UserId))
                                .ToList());

                context.SaveChanges();
            }

            foreach (var sessionUser in model.Guests)
            {
                context.UserSessions.Add(new UserSession
                {
                    SessionId = session.Id,
                    UserId = sessionUser
                });

                context.SaveChanges();
            }

            return session;
        }
    }
}
