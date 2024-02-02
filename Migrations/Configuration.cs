
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BinusZoom.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BinusZoom.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}