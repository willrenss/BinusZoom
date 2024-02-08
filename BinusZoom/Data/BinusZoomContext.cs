using BinusZoom.Models;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Data;

public class BinusZoomContext : DbContext
{
    public BinusZoomContext(DbContextOptions<BinusZoomContext> options)
        : base(options)
    { 
        //ensure migrated
        Database.EnsureCreated();
    }

    public DbSet<Meeting> Meeting { get; set; } = default!;

public DbSet<BinusZoom.Models.Registration> Registration { get; set; } = default!;
}