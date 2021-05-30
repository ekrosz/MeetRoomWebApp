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

        public TimeSpan SessionDuration { get; set; }

        public List<UserViewModel> Guests { get; set; }
    }
}
