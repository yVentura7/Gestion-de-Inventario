using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Caso1_Tarea.Models.Enums;

namespace Caso1_Tarea.Models;

public partial class Lab5Context : DbContext
{
    public Lab5Context()
    {
    }

    public Lab5Context(DbContextOptions<Lab5Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Detallepedido> Detallepedidos { get; set; }

    public virtual DbSet<Movimientosinventario> Movimientosinventarios { get; set; }

    public virtual DbSet<Pedidosproveedore> Pedidosproveedores { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=lab5;Username=postgres;Password=admin",
                o => o.MapEnum<TipoMovimiento>("tipo_movimiento")
                    .MapEnum<EstadoPedido>("estado_pedido"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("estado_pedido", new[] { "Pendiente", "Enviado", "Entregado" })
            .HasPostgresEnum("tipo_movimiento", new[] { "Entrada", "Salida" });

        modelBuilder.Entity<Detallepedido>(entity =>
        {
            entity.HasKey(e => e.Detalleid).HasName("detallepedidos_pkey");

            entity.ToTable("detallepedidos");

            entity.Property(e => e.Detalleid).HasColumnName("detalleid");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Pedidoid).HasColumnName("pedidoid");
            entity.Property(e => e.Productoid).HasColumnName("productoid");

            entity.HasOne(d => d.Pedido).WithMany(p => p.Detallepedidos)
                .HasForeignKey(d => d.Pedidoid)
                .HasConstraintName("detallepedidos_pedidoid_fkey");

            entity.HasOne(d => d.Producto).WithMany(p => p.Detallepedidos)
                .HasForeignKey(d => d.Productoid)
                .HasConstraintName("detallepedidos_productoid_fkey");
        });

        modelBuilder.Entity<Movimientosinventario>(entity =>
        {
            entity.Property(e => e.Tipomovimiento)
                .HasColumnName("tipomovimiento")
                .HasColumnType("tipo_movimiento");
            entity.HasKey(e => e.Movimientoid).HasName("movimientosinventario_pkey");

            entity.ToTable("movimientosinventario");

            entity.Property(e => e.Movimientoid).HasColumnName("movimientoid");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Productoid).HasColumnName("productoid");

            entity.HasOne(d => d.Producto).WithMany(p => p.Movimientosinventarios)
                .HasForeignKey(d => d.Productoid)
                .HasConstraintName("movimientosinventario_productoid_fkey");
        });

        modelBuilder.Entity<Pedidosproveedore>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasColumnType("estado_pedido");
            entity.HasKey(e => e.Pedidoid).HasName("pedidosproveedores_pkey");

            entity.ToTable("pedidosproveedores");

            entity.Property(e => e.Pedidoid).HasColumnName("pedidoid");
            entity.Property(e => e.Fechapedido).HasColumnName("fechapedido");
            entity.Property(e => e.Proveedorid).HasColumnName("proveedorid");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Pedidosproveedores)
                .HasForeignKey(d => d.Proveedorid)
                .HasConstraintName("pedidosproveedores_proveedorid_fkey");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Productoid).HasName("productos_pkey");

            entity.ToTable("productos");

            entity.Property(e => e.Productoid).HasColumnName("productoid");
            entity.Property(e => e.Cantidadinventario).HasColumnName("cantidadinventario");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.Stockminimo)
                .HasDefaultValue(10)
                .HasColumnName("stockminimo");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.Proveedorid).HasName("proveedores_pkey");

            entity.ToTable("proveedores");

            entity.Property(e => e.Proveedorid).HasColumnName("proveedorid");
            entity.Property(e => e.Contacto)
                .HasMaxLength(255)
                .HasColumnName("contacto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
