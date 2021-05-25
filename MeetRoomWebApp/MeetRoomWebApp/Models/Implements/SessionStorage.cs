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
                        UserSessions = rec.ClientSessions.ToDictionary(rec => rec.User.Id, rec => rec.User.Email)
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
                        UserSessions = rec.ClientSessions.ToDictionary(rec => rec.User.Id, rec => rec.User.Email)
                    })
                    .ToList();
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
                        if (model.DateSession.Date == model.DateSession.AddMinutes(model.SessionDuration).Date)
                        {
                            CreateModel(model, new Session(), context);
                            context.SaveChanges();
                        }
                        else
                        {
                            var firstModel = new SessionBindingModel
                            {
                                DateSession = model.DateSession,
                                SessionDuration = Convert.ToInt32((model.DateSession.AddDays(1).Date - model.DateSession).TotalMinutes),
                                UserSessions = model.UserSessions
                            };
                            var secondModel = new SessionBindingModel
                            {
                                DateSession = model.DateSession.AddDays(1).Date,
                                SessionDuration = model.SessionDuration - firstModel.SessionDuration,
                                UserSessions = model.UserSessions
                            };

                            CreateModel(firstModel, new Session(), context);
                            CreateModel(secondModel, new Session(), context);
                            context.SaveChanges();
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
        }

        private Session CreateModel(SessionBindingModel model, Session session, MeetRoomDbContext context)
        {
            var otherSessions = context.Sessions
                .Where(rec => rec.DateSession.Date == model.DateSession.Date || rec.DateSession.Date == model.DateSession.AddDays(1).Date)
                .OrderBy(rec => rec.DateSession);

            foreach(var item in otherSessions)
            {
                if(model.DateSession < item.DateSession && model.DateSession.AddMinutes(model.SessionDuration) > item.DateSession ||
                    model.DateSession < item.DateSession.AddMinutes(item.SessionDurationInMinutes) && model.DateSession.AddMinutes(model.SessionDuration) > item.DateSession.AddMinutes(item.SessionDurationInMinutes) ||
                    model.DateSession > item.DateSession && model.DateSession.AddMinutes(model.SessionDuration) < item.DateSession.AddMinutes(item.SessionDurationInMinutes))
                {
                    throw new Exception("В этом промежутке времени есть бронь");
                }
            }

            session.DateSession = model.DateSession;
            session.SessionDurationInMinutes = model.SessionDuration;

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
