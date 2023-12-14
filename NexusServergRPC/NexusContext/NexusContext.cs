namespace NexusServergRPC.NexusContext;

using Microsoft.EntityFrameworkCore;
using NexusServergRPC.Entity;

public class NexusContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Contact> Contacts { get; set; }

    public NexusContext(DbContextOptions<NexusContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}