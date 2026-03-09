using IIS_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace IIS_backend.DataAccess;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Day> Days => Set<Day>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<CompetitionCurrency> CompetitionCurrencies => Set<CompetitionCurrency>();

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketItem> TicketItems => Set<TicketItem>();
    public DbSet<PaymentSnapshot> PaymentSnapshots => Set<PaymentSnapshot>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<TicketCreationRequestError> TicketCreationRequestErrors => Set<TicketCreationRequestError>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Competition
        modelBuilder.Entity<Competition>()
            .HasMany(x => x.Days)
            .WithOne(x => x.Competition)
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Competition>()
            .HasMany(x => x.Zones)
            .WithOne(x => x.Competition)
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Currency 
        modelBuilder.Entity<Currency>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Currency>().HasIndex(x => x.CurrencyName).IsUnique();

        // CompCur
        modelBuilder.Entity<CompetitionCurrency>()
            .HasKey(x => new { x.CompetitionId, x.CurrencyId });

        modelBuilder.Entity<CompetitionCurrency>()
            .HasOne(x => x.Competition)
            .WithMany(x => x.CompetitionCurrencies)
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompetitionCurrency>()
            .HasOne(x => x.Currency)
            .WithMany(x => x.CompetitionCurrencies)
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Day
        modelBuilder.Entity<Day>()
            .HasIndex(x => new { x.CompetitionId, x.Date })
            .IsUnique();

        // Zone
        modelBuilder.Entity<Zone>()
            .HasIndex(x => new { x.CompetitionId, x.Characteristics })
            .IsUnique();

        // Ticket code unique
        modelBuilder.Entity<Ticket>()
            .HasIndex(x => x.TicketCode)
            .IsUnique();

        // TicketItem
        modelBuilder.Entity<TicketItem>()
            .HasIndex(x => new { x.TicketId, x.DayId })
            .IsUnique();

        modelBuilder.Entity<TicketItem>()
            .HasOne(x => x.Ticket)
            .WithMany(x => x.TicketItems)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketItem>()
            .HasOne(x => x.Day)
            .WithMany(x => x.TicketItems)
            .HasForeignKey(x => x.DayId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketItem>()
            .HasOne(x => x.Zone)
            .WithMany(x => x.TicketItems)
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // PaymentSnapshot 
        modelBuilder.Entity<Ticket>()
            .HasOne(x => x.PaymentSnapshot)
            .WithMany()
            .HasForeignKey(x => x.PaymentSnapshotId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentSnapshot>()
            .HasOne(x => x.SelectedCurrency)
            .WithMany()
            .HasForeignKey(x => x.SelectedCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        // PromoCode 
        modelBuilder.Entity<PromoCode>()
            .HasIndex(x => x.Code)
            .IsUnique();

        // issued - used
        modelBuilder.Entity<Ticket>()
            .HasOne(x => x.IssuedPromoCode)
            .WithOne(x => x.IssuedTicket)
            .HasForeignKey<Ticket>(x => x.IssuedPromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(x => x.UsedPromoCode)
            .WithOne(x => x.UsedTicket)
            .HasForeignKey<Ticket>(x => x.UsedPromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Error table index for quick lookup
        modelBuilder.Entity<TicketCreationRequestError>()
            .HasIndex(x => new { x.Email, x.TicketCode })
            .IsUnique();
    }
}