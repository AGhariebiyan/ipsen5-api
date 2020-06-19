using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMAPI.Models;
using Microsoft.Extensions.Logging;

namespace GMAPI.Models
{
    public class PostgresDatabaseContext : DbContext
    {
        public PostgresDatabaseContext(DbContextOptions<PostgresDatabaseContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<WorksAt> WorksAt { get; set; }
        public DbSet<Attended> Attended { get; set; }
        public DbSet<FieldOfStudy> Studies { get; set; } 
        public DbSet<Image> Image { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<GMAPI.Models.Event> Event { get; set; }
        public DbSet<Verification> Verifications { get; set; }

        public DbSet<GMAPI.Models.Participant> Participant { get; set; }

        public DbSet<GMAPI.Models.News> News { get; set; }

        public DbSet<Role> Role { get; set; }

        public DbSet<Article> Article { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>().HasQueryFilter(p => p.Active);
        }
    }
}
