using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetRoomWebApp.Data.Entities
{
    /// <summary>
    /// Database Entity Model - Session
    /// </summary>
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateSession { get; set; }

        [Required]
        public TimeSpan SessionDuration { get; set; }

        [ForeignKey("SessionId")]
        public virtual List<UserSession> UserSessions { get; set; }
    }
}
