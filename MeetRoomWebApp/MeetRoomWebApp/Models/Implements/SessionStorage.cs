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
    public class SessionStorage : ISessionStorage
    {
        public List<SessionViewModel> GetFullList()
        {
            using (var context = new MeetRoomDbContext())
            {
                return context.Sessions
                    .Include(rec => rec.ClientSessions)
                    .ThenInclude(rec => rec.User)
                    .Select(rec => new SessionViewModel
                    {
                        Id = rec.Id,
                        DateSession = rec.DateSession,
                        SessionDurationInMinutes = rec.SessionDurationInMinutes,
                        ClientSessions = rec.ClientSessions.ToDictionary(rec => rec.User.Id, rec => rec.User.Email)
                    }).ToList();
            }
        }

        public List<SessionViewModel> GetFilteredList(SessionBindingModel model)
        {
            using (var context = new MeetRoomDbContext())
            {
                return context.Sessions
                    .Include(rec => rec.ClientSessions)
                    .ThenInclude(rec => rec.User)
                    .Where(rec => rec.DateSession.Date == model.DateSession.Date)
                    .ToList()
                    .Select(rec => new SessionViewModel
                    {
                        Id = rec.Id,
                        DateSession = rec.DateSession,
                        SessionDurationInMinutes = rec.SessionDurationInMinutes,
                        ClientSessions = rec.ClientSessions.ToDictionary(rec => rec.User.Id, rec => rec.User.Email)
                    })
                    .ToList();
            }
        }

        public SessionViewModel GetElement(SessionBindingModel model)
        {
            if (model == null)
            {
                return null;
            }

            using (var context = new MeetRoomDbContext())
            {
                var session = context.Sessions
                    .Include(rec => rec.ClientSessions)
                    .FirstOrDefault(rec => rec.Id == model.Id);

                return session != null ? new SessionViewModel
                { 
                    Id = session.Id,
                    DateSession = session.DateSession,
                    SessionDurationInMinutes = session.SessionDurationInMinutes,
                    ClientSessions = session.ClientSessions.ToDictionary(rec => rec.User.Id, rec => rec.User.Email)
                } :
                null;
            }
        }

        public void Insert(SessionBindingModel model)
        {
            using (var context = new MeetRoomDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        CreateModel(model, new Session(), context);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

        public void Edit(SessionBindingModel model)
        {
            using (var context = new MeetRoomDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var element = context.Sessions
                            .FirstOrDefault(rec => rec.Id == model.Id);

                        if (element == null)
                        {
                            throw new Exception("Элемент не найден");
                        }

                        CreateModel(model, element, context);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

        public void Delete(SessionBindingModel model)
        {
            using (var context = new MeetRoomDbContext())
            {
                Session element = context.Sessions
                    .FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Sessions.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }

        private Session CreateModel(SessionBindingModel model, Session session, MeetRoomDbContext context)
        {
            session.DateSession = model.DateSession;
            session.SessionDurationInMinutes = model.SessionDurationInMinutes;

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
                        .Where(rec => !model.UserSessions
                            .ContainsKey(rec.UserId))
                                .ToList());

                context.SaveChanges();
            }

            foreach (var sessionUser in model.UserSessions)
            {
                context.UserSessions.Add(new UserSession
                {
                    SessionId = session.Id,
                    UserId = sessionUser.Key
                });

                context.SaveChanges();
            }

            return session;
        }
    }
}
