using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OdeToFood.Domain;

namespace OdeToFood.Data
{
    internal class OdeToFoodContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public OdeToFoodContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Restaurant>().HasData(new List<Restaurant>
            {
                new Restaurant
                {
                    Id = 1,
                    Name = "Wok palace",
                    City = "Hasselt",
                    Country = "Belgium"
                }
            });
        }
    }
}
