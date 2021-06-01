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
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Session start date and time
        /// </summary>
        [Required]
        public DateTime DateSession { get; set; }

        /// <summary>
        /// Session duration
        /// </summary>
        [Required]
        public TimeSpan SessionDuration { get; set; }

        /// <summary>
        /// Session foreign key
        /// </summary>
        [ForeignKey("SessionId")]
        public virtual List<UserSession> UserSessions { get; set; }
    }
}
