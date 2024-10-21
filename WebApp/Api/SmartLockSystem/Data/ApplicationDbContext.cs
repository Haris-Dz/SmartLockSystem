using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data.Models;

namespace SmartLockSystem.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<LockEvent> LockEvents { get; set; }


    public ApplicationDbContext(
        DbContextOptions options) : base(options)
    {


    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }


    }
}