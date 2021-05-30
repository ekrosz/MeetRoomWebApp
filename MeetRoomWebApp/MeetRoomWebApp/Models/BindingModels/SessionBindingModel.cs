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
        public int? Id { get; set; }

        [CustomDateTime(ErrorMessage = "Incorrect start date of booking (00 or 30 minutes) or the selected date is less than the current date!")]
        public DateTime DateSession { get; set; }

        [CustomTimeSpan(ErrorMessage = "Incorrect booking duration (00 or 30 minutes)!")]
        public TimeSpan SessionDuration { get; set; }

        public List<string> Guests { get; set; }
    }
}
