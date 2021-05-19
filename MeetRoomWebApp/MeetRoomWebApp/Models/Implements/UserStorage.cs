using System;
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

        public UserViewModel GetElement(string user)
        {
            using (var context = new MeetRoomDbContext())
            {
                var result = context.Users
                     .FirstOrDefault(rec => rec.Id == user || rec.UserName == user);

                return user != null ? new UserViewModel
                {
                    Id = result.Id,
                    Email = result.Email
                } :
                null;
            }
        }
    }
}
