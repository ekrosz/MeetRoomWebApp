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

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Session> Sessions { get; set; }

        public virtual DbSet<UserSession> UserSessions { get; set; }

        public virtual DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
    }
}
