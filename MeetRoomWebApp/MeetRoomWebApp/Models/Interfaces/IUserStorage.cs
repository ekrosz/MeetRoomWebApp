using MeetRoomWebApp.Models.ViewModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    /// <summary>
    /// Interface with methods of interaction with the database of the "User" model
    /// </summary>
    public interface IUserStorage
    {
        public List<UserViewModel> GetFullList();

        public UserViewModel GetElement(string user);
    }
}
