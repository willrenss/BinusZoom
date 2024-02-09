using BinusZoom.Models;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Data;

public class BinusZoomContext : DbContext
{
    public BinusZoomContext(DbContextOptions<BinusZoomContext> options)
        : base(options)
    { 
        //ensure migrated
    }

    public DbSet<Meeting> Meeting { get; set; } = default!;

    public DbSet<Registration> Registration { get; set; } = default!;
}