using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Models;

public partial class ProyectoIntegradorContext : DbContext
{
    public ProyectoIntegradorContext()
    {
    }

    public ProyectoIntegradorContext(DbContextOptions<ProyectoIntegradorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Detalle> Detalles { get; set; }

    public virtual DbSet<Ordene> Ordenes { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<NavItem> NavItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Detalle>(entity =>
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasKey(r => r.IdRol);

            entity.HasKey(e => e.Id).HasName("PK__detalle__3213E83F249120FD");

            entity.ToTable("detalle");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.OrdenId).HasColumnName("orden_id");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.Total).HasColumnName("total");

            entity.HasOne(d => d.Orden).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__detalle__orden_i__3D5E1FD2");

            entity.HasOne(d => d.Producto).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__detalle__product__3E52440B");
        });

        modelBuilder.Entity<Ordene>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ordenes__3213E83FFD3DE6A9");

            entity.ToTable("ordenes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(6)
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaRecibida)
                .HasPrecision(6)
                .HasColumnName("fecha_recibida");
            entity.Property(e => e.Numero)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("numero");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Ordenes)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__ordenes__usuario__3F466844");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__producto__3213E83FFD4433AD");

            entity.ToTable("productos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("imagen");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Productos)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__productos__usuar__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F4BF52D8A");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Telefono)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.Tipo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.IdRol).HasColumnName("idRol")
                .ValueGeneratedOnAdd();
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.IdRol);


        });

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
