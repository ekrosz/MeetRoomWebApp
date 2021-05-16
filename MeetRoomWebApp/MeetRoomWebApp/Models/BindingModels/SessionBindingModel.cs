using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MeetRoomWebApp.Models.BindingModels
{
    public class SessionBindingModel
    {
        public int? Id { get; set; }

        [DisplayName("Дата брони")]
        public DateTime DateSession { get; set; }

        [DisplayName("Длительность")]
        public int SessionDuration { get; set; }

        public Dictionary<string, string> UserSessions { get; set; }
    }
}
