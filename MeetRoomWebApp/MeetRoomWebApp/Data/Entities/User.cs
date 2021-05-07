using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetRoomWebApp.Data.Entities
{
    public class User : IdentityUser
    {
        [ForeignKey("UserId")]
        public virtual List<UserSession> UserSessions { get; set; }
    }
}
