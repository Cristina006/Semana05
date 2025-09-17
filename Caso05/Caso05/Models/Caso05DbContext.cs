using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Caso05.Models;

public partial class Caso05DbContext : DbContext
{
    public Caso05DbContext()
    {
    }

    public Caso05DbContext(DbContextOptions<Caso05DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Detallesventum> Detallesventa { get; set; }

    public virtual DbSet<Movimientosinventario> Movimientosinventarios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Solo usar la cadena de conexión por defecto si no se ha configurado externamente
            optionsBuilder.UseNpgsql("Host=localhost;Database=Caso05;Username=postgres;Password=postgre123;Port=5432");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Categoriaid).HasName("categorias_pkey");

            entity.ToTable("categorias");

            entity.HasIndex(e => e.Nombre, "categorias_nombre_key").IsUnique();

            entity.Property(e => e.Categoriaid).HasColumnName("categoriaid");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Clienteid).HasName("clientes_pkey");

            entity.ToTable("clientes");

            entity.HasIndex(e => e.Email, "clientes_email_key").IsUnique();

            entity.Property(e => e.Clienteid).HasColumnName("clienteid");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Direccionenvio)
                .HasMaxLength(255)
                .HasColumnName("direccionenvio");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Detallesventum>(entity =>
        {
            entity.HasKey(e => e.Detalleventaid).HasName("detallesventa_pkey");

            entity.ToTable("detallesventa");

            entity.Property(e => e.Detalleventaid).HasColumnName("detalleventaid");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Preciounitario)
                .HasPrecision(18, 2)
                .HasColumnName("preciounitario");
            entity.Property(e => e.Productoid).HasColumnName("productoid");
            entity.Property(e => e.Ventaid).HasColumnName("ventaid");

            entity.HasOne(d => d.Producto).WithMany(p => p.Detallesventa)
                .HasForeignKey(d => d.Productoid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detallesventa_productos");

            entity.HasOne(d => d.Venta).WithMany(p => p.Detallesventa)
                .HasForeignKey(d => d.Ventaid)
                .HasConstraintName("fk_detallesventa_ventas");
        });

        modelBuilder.Entity<Movimientosinventario>(entity =>
        {
            entity.HasKey(e => e.Movimientoid).HasName("movimientosinventario_pkey");

            entity.ToTable("movimientosinventario");

            entity.Property(e => e.Movimientoid).HasColumnName("movimientoid");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Productoid).HasColumnName("productoid");
            entity.Property(e => e.Referenciaid).HasColumnName("referenciaid");
            entity.Property(e => e.Tipomovimiento)
                .HasMaxLength(20)
                .HasColumnName("tipomovimiento");

            entity.HasOne(d => d.Producto).WithMany(p => p.Movimientosinventarios)
                .HasForeignKey(d => d.Productoid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_movimientosinventario_productos");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Productoid).HasName("productos_pkey");

            entity.ToTable("productos");

            entity.Property(e => e.Productoid).HasColumnName("productoid");
            entity.Property(e => e.Categoriaid).HasColumnName("categoriaid");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(18, 2)
                .HasColumnName("precio");
            entity.Property(e => e.Proveedorid).HasColumnName("proveedorid");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Stockminimo)
                .HasDefaultValue(5)
                .HasColumnName("stockminimo");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.Categoriaid)
                .HasConstraintName("fk_productos_categorias");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos)
                .HasForeignKey(d => d.Proveedorid)
                .HasConstraintName("fk_productos_proveedores");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.Proveedorid).HasName("proveedores_pkey");

            entity.ToTable("proveedores");

            entity.HasIndex(e => e.Email, "proveedores_email_key").IsUnique();

            entity.Property(e => e.Proveedorid).HasColumnName("proveedorid");
            entity.Property(e => e.Contacto)
                .HasMaxLength(100)
                .HasColumnName("contacto");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombreempresa)
                .HasMaxLength(100)
                .HasColumnName("nombreempresa");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Ventaid).HasName("ventas_pkey");

            entity.ToTable("ventas");

            entity.Property(e => e.Ventaid).HasColumnName("ventaid");
            entity.Property(e => e.Clienteid).HasColumnName("clienteid");
            entity.Property(e => e.Fechaventa)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fechaventa");
            entity.Property(e => e.Total)
                .HasPrecision(18, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Venta)
                .HasForeignKey(d => d.Clienteid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ventas_clientes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
