using MeetRoomWebApp.Models.ViewModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    public interface IRoomStorage
    {
        public List<RoomViewModel> GetFullList();
    }
}
