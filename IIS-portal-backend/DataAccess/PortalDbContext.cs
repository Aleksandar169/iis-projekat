namespace IIS_portal_backend.DataAccess
{
  
    using Microsoft.EntityFrameworkCore;
  

    namespace IIS_portal_backend.Data
    {
        public class PortalDbContext : DbContext
        {
            public PortalDbContext(DbContextOptions<PortalDbContext> options)
                : base(options)
            {
            }

            public DbSet<TicketSale> TicketSales { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                
                modelBuilder.Entity<TicketSale>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.CompetitionDay).HasColumnType("date");
                    entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
                });
            }
        }
    }

}
