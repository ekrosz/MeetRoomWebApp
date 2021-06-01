using MeetRoomWebApp.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MeetRoomWebApp.Models.BindingModels
{
    /// <summary>
    /// Binding model for "Session" entity 
    /// </summary>
    public class SessionBindingModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Session start date and time
        /// </summary>
        [CustomDateTime(ErrorMessage = "Incorrect start date of booking (00 or 30 minutes) or the selected date is less than the current date!")]
        public DateTime DateSession { get; set; }

        /// <summary>
        /// Session duration
        /// </summary>
        [CustomTimeSpan(ErrorMessage = "Incorrect booking duration (00 or 30 minutes)!")]
        public TimeSpan SessionDuration { get; set; }

        /// <summary>
        /// Guest ID list
        /// </summary>
        public List<string> Guests { get; set; }
    }
}
