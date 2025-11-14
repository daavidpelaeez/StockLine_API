using Microsoft.EntityFrameworkCore;
using StockLine_API.Models;

namespace StockLine_API
{
    public class StockLineContext : DbContext
    {
        public StockLineContext(DbContextOptions<StockLineContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ayuntamiento> Ayuntamientos { get; set; }
        public DbSet<Comercial> Comerciales { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<SIM> SIMs { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<EnvioDetalle> EnviosDetalle { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Role>().HasKey(r => r.RoleID);
            modelBuilder.Entity<Usuario>().HasKey(u => u.UsuarioID);
            modelBuilder.Entity<Ayuntamiento>().HasKey(a => a.AyuntamientoID);
            modelBuilder.Entity<Comercial>().HasKey(c => c.ComercialID);
            modelBuilder.Entity<Producto>().HasKey(p => p.ProductoID);
            modelBuilder.Entity<SIM>().HasKey(s => s.SIMID);
            modelBuilder.Entity<Envio>().HasKey(e => e.EnvioID);
            modelBuilder.Entity<EnvioDetalle>().HasKey(ed => ed.EnvioDetalleID);
            modelBuilder.Entity<MovimientoStock>().HasKey(ms => ms.MovimientoID);


            
            modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ayuntamiento>()
                .HasOne(a => a.Comercial)
                .WithMany()
                .HasForeignKey(a => a.ComercialID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Ayuntamiento)
                .WithMany(a => a.Envios)
                .HasForeignKey(e => e.AyuntamientoID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Comercial)
                .WithMany(c => c.Envios)
                .HasForeignKey(e => e.ComercialID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.UsuarioModificador)
                .WithMany()
                .HasForeignKey(e => e.UsuarioModificadorID)
                .IsRequired(false);

            modelBuilder.Entity<SIM>()
                .HasOne(s => s.Producto)
                .WithMany()
                .HasForeignKey(s => s.ProductoID)
                .IsRequired(false);

            modelBuilder.Entity<EnvioDetalle>()
                .HasOne(ed => ed.Envio)
                .WithMany(e => e.Detalles)
                .HasForeignKey(ed => ed.EnvioID);

            modelBuilder.Entity<EnvioDetalle>()
                .HasOne(ed => ed.Producto)
                .WithMany()
                .HasForeignKey(ed => ed.ProductoID);

            modelBuilder.Entity<EnvioDetalle>()
                .HasOne(ed => ed.SIM)
                .WithMany()
                .HasForeignKey(ed => ed.SIMID)
                .IsRequired(false);

            modelBuilder.Entity<MovimientoStock>()
                .HasOne(ms => ms.Producto)
                .WithMany(p => p.MovimientosStock)
                .HasForeignKey(ms => ms.ProductoID)
                .OnDelete(DeleteBehavior.SetNull); 

            modelBuilder.Entity<MovimientoStock>()
                .HasOne(ms => ms.Usuario)
                .WithMany(u => u.MovimientosStock)
                .HasForeignKey(ms => ms.UsuarioID);

            modelBuilder.Entity<EnvioDetalle>()
                .HasOne(ed => ed.Producto)
                .WithMany(p => p.EnvioDetalles) 
                .HasForeignKey(ed => ed.ProductoID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
