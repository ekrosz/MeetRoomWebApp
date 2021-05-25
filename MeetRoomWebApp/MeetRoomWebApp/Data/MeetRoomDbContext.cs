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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=MeetRoomDatabase;User Id=postgres;Password=1234;");
            }

            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Session> Sessions { get; set; }

        public virtual DbSet<UserSession> UserSessions { get; set; }

        public virtual DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
    }
}
