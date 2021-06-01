using System;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.ViewModels
{
    /// <summary>
    /// Model-view "Session" for displaying to the user
    /// </summary>
    public class SessionViewModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Session start date and time
        /// </summary>
        public DateTime DateSession { get; set; }

        /// <summary>
        /// Session duration
        /// </summary>
        public TimeSpan SessionDuration { get; set; }

        /// <summary>
        /// Guest view list
        /// </summary>
        public List<UserViewModel> Guests { get; set; }
    }
}
