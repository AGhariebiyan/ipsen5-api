using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMAPI.Models;

namespace GMAPI.Models
{
    public class PostgresDatabaseContext: DbContext
    { 
        public PostgresDatabaseContext(DbContextOptions<PostgresDatabaseContext> options):base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }

        public DbSet<Newspost> Newspost { get; set; }

    }
}
