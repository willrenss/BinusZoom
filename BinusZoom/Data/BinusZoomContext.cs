using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BinusZoom.Models;

namespace BinusZoom.Data
{
    public class BinusZoomContext : DbContext
    {
        public BinusZoomContext (DbContextOptions<BinusZoomContext> options)
            : base(options)
        {
        }

        public DbSet<BinusZoom.Models.Meeting> Meeting { get; set; } = default!;
    }
}
