using System.Collections.Generic;
using System.Linq;
using MeetRoomWebApp.Data;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;

namespace MeetRoomWebApp.Models.Implements
{
    public class UserStorage : IUserStorage
    {
        public List<UserViewModel> GetFullList()
        {
            using(var context = new MeetRoomDbContext())
            {
                return context.Users.Select(rec => new UserViewModel
                {
                    Id = rec.Id,
                    Email = rec.Email,
                }).ToList();
            }
        }

        public List<UserViewModel> GetFilteredList(int sessionId)
        {
            using (var context = new MeetRoomDbContext())
            {
                var sessionUsers = context.UserSessions
                    .Where(rec => rec.Id == sessionId)
                    .ToList();

                List<UserViewModel> resultList = new List<UserViewModel>();

                sessionUsers.ForEach(i => resultList.Add((UserViewModel)context.Users
                    .Where(rec => rec.Id == i.UserId)
                    .Select(rec => new UserViewModel 
                    {
                        Id = rec.Id, 
                        Email = rec.Email,
                    })));

                return resultList;
            }
        }
    }
}
