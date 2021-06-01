using System;
using System.Collections.Generic;
using System.Linq;
using MeetRoomWebApp.Data;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;

namespace MeetRoomWebApp.Models.Implements
{
    /// <summary>
    /// IUserStorage interface implementation class
    /// </summary>
    public class UserStorage : IUserStorage
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly MeetRoomDbContext _context;

        public UserStorage(MeetRoomDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Full list of users
        /// </summary>
        /// <returns>Session list</returns>
        public List<UserViewModel> GetFullList()
        {
            return _context.Users.Select(rec => new UserViewModel
            {
                Id = rec.Id,
                Email = rec.Email,
            }).ToList();
        }

        /// <summary>
        /// Search for a session by ID
        /// </summary>
        /// <param name="user">User's ID or Email</param>
        /// <returns>User</returns>
        public UserViewModel GetElement(string user)
        {
            var result = _context.Users
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
