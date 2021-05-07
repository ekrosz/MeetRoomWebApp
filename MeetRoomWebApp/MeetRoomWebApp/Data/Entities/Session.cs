using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetRoomWebApp.Data.Entities
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }

        public virtual Room Room { get; set; }

        [Required]
        public DateTime DateSession { get; set; }

        [Required]
        public int SessionDurationInMinutes { get; set; }

        [ForeignKey("SessionId")]
        public virtual List<UserSession> ClientSessions { get; set; }
    }
}
