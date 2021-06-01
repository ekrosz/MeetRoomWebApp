using Microsoft.EntityFrameworkCore;
using MeetRoomWebApp.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MeetRoomWebApp.Data
{
    /// <summary>
    /// The main class for connecting to the database
    /// </summary>
    public class MeetRoomDbContext : DbContext
    {
        public MeetRoomDbContext(DbContextOptions<MeetRoomDbContext> options)
        : base(options) { }

        /// <summary>
        /// User entity
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        /// Session entity
        /// </summary>
        public virtual DbSet<Session> Sessions { get; set; }

        /// <summary>
        /// UserSession entity
        /// </summary>
        public virtual DbSet<UserSession> UserSessions { get; set; }

        /// <summary>
        /// IdentityUserClaims entity
        /// </summary>
        public virtual DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
    }
}
