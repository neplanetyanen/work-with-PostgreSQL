using Microsoft.EntityFrameworkCore;
using DatabaseModels;

namespace DatabaseContext;

public class TicketCenterContext : DbContext
{
    //расписать все таблички
    public DbSet<Exhibition> Exhibitions { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exhibition>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Exhibition>()
            .Property(e => e.Name)
            .IsRequired();
        modelBuilder.Entity<Exhibition>()
            .Property(e => e.Date)
            .IsRequired();
        modelBuilder.Entity<Exhibition>()
            .HasMany(e => e.Tickets)
            .WithOne(t => t.Exhibition)
            .HasForeignKey(t => t.ExhibitionId)
            .IsRequired();
        
        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Price)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.PriceWithDiscount)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Exhibition)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.ExhibitionId)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Visitor)
            .WithMany(v => v.Tickets)
            .HasForeignKey(t => t.VisitorId)
            .IsRequired();
        
        modelBuilder.Entity<Visitor>()
            .HasKey(v => v.Id);
        modelBuilder.Entity<Visitor>()
            .Property(v => v.Name)
            .IsRequired();
        modelBuilder.Entity<Visitor>()
            .Property(v => v.Discount)
            .IsRequired();
        modelBuilder.Entity<Visitor>()
            .HasMany(v => v.Tickets)
            .WithOne(t => t.Visitor)
            .HasForeignKey(t => t.VisitorId)
            .IsRequired();
        
        base.OnModelCreating(modelBuilder);
    }

    public TicketCenterContext() : base(
        GetOptions("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=12345"))
    {
        //Database.EnsureCreated();
    }
    
    private static DbContextOptions GetOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TicketCenterContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return optionsBuilder.Options;
    }
}
