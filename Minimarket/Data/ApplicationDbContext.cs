using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using System.Collections.Generic;

namespace Minimarket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<NavItem> NavItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasKey(r => r.IdRol);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => rp.Id);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Roles)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.IdRol);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permisos)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.IdPermisos);
        }


    }
}
