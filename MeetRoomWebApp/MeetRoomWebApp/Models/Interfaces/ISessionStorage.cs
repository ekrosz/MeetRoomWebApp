using MeetRoomWebApp.Models.ViewModels;
using MeetRoomWebApp.Models.BindingModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    /// <summary>
    /// Interface with methods of interaction with the database of the "Session" model
    /// </summary>
    public interface ISessionStorage
    {
        public List<SessionViewModel> GetFullList();

        public List<SessionViewModel> GetFilteredList(SessionBindingModel model);

        public void Insert(SessionBindingModel model);
    }
}
