using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ShoeVault.Models;
using System.Collections.Generic;


namespace ShoeVault.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shoe> Shoes { get; set; }

        public DbSet<ShoeImage> ShoeImages { get; set; }

    }
}
