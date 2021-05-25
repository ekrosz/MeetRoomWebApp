using System;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.ViewModels
{
    /// <summary>
    /// Model-view "Session" for displaying to the user
    /// </summary>
    public class SessionViewModel
    {
        public int Id { get; set; }

        public DateTime DateSession { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public Dictionary<string, string> UserSessions { get; set; }
    }
}
