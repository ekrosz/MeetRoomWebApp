using System;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.ViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public DateTime DateSession { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public Dictionary<string, string> ClientSessions { get; set; }
    }
}
