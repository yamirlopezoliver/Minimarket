using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using System.Collections.Generic;

namespace Minimarket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
