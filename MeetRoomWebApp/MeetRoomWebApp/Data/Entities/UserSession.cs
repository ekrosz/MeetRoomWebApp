using System.ComponentModel.DataAnnotations;

namespace MeetRoomWebApp.Data.Entities
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        public int SessionId { get; set; }

        public virtual Session Session { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
