using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetRoomWebApp.Data;
using MeetRoomWebApp.Models.Interfaces;
using MeetRoomWebApp.Models.ViewModels;

namespace MeetRoomWebApp.Models.Implements
{
    public class RoomStorage : IRoomStorage
    {
        public List<RoomViewModel> GetFullList()
        {
            using (var context = new MeetRoomDbContext())
            {
                return context.Rooms.Select(rec => new RoomViewModel
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    Capacity = rec.Capacity
                }).ToList();
            }
        }
    }
}
