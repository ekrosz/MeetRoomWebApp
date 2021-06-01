using MeetRoomWebApp.Models.ViewModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    /// <summary>
    /// Interface with methods of interaction with the database of the "User" model
    /// </summary>
    public interface IUserStorage
    {
        /// <summary>
        /// Full list of users
        /// </summary>
        /// <returns>Session list</returns>
        public List<UserViewModel> GetFullList();

        /// <summary>
        /// Search for a session by ID
        /// </summary>
        /// <param name="user">User's ID or Email</param>
        /// <returns>User</returns>
        public UserViewModel GetElement(string user);
    }
}
