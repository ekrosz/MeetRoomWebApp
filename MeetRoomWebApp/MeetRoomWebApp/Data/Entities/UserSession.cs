using System.ComponentModel.DataAnnotations;

namespace MeetRoomWebApp.Data.Entities
{
    /// <summary>
    /// Database Entity Model for Implementing Many-to-Many Relationships (User-Sission)
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Session ID
        /// </summary>
        public int SessionId { get; set; }

        public virtual Session Session { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
