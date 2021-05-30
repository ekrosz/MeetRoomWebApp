using MeetRoomWebApp.Models.ViewModels;
using MeetRoomWebApp.Models.BindingModels;
using System.Collections.Generic;
using System;

namespace MeetRoomWebApp.Models.Interfaces
{
    /// <summary>
    /// Interface with methods of interaction with the database of the "Session" model
    /// </summary>
    public interface ISessionStorage
    {
        public List<SessionViewModel> GetFullList();

        public List<SessionViewModel> GetFilteredListByWeek(DateTime dateFrom, DateTime dateTo);

        public List<SessionViewModel> GetFilteredListByUser(string user);

        public SessionViewModel GetElement(SessionBindingModel model);

        public void Insert(SessionBindingModel model);

        public void Update(SessionBindingModel model);

        public void Delete(SessionBindingModel model);
    }
}
