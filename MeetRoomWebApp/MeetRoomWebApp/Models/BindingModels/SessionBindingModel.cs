using System;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.BindingModels
{
    public class SessionBindingModel
    {
        public int? Id { get; set; }

        public DateTime DateSession { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public Dictionary<string, string> UserSessions { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
