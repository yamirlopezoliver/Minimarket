using Microsoft.EntityFrameworkCore;
using Minimarket.Models;

namespace Minimarket.Data
{
    public class MinimarketContext : DbContext
    {
        public MinimarketContext(DbContextOptions<MinimarketContext> options)
            : base(options)
        {
        }

        public DbSet<Productos> Productos { get; set; }
    }
}
