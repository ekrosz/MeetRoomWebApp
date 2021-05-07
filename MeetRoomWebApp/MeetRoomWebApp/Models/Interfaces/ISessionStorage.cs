using MeetRoomWebApp.Models.ViewModels;
using MeetRoomWebApp.Models.BindingModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    public interface ISessionStorage
    {
        public List<SessionViewModel> GetFullList();

        public List<SessionViewModel> GetFilteredList(SessionBindingModel model);

        public SessionViewModel GetElement(SessionBindingModel model);

        public void Insert(SessionBindingModel model);

        public void Edit(SessionBindingModel model);

        public void Delete(SessionBindingModel model);
    }
}
