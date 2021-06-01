using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetRoomWebApp.Data.Entities
{
    /// <summary>
    /// Database Entity Model - User
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// User foreign key
        /// </summary>
        [ForeignKey("UserId")]
        public virtual List<UserSession> UserSessions { get; set; }
    }
}
