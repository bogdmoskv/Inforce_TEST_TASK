using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Inforce_.NET_Task_Moskvichev_Bogdan
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
   
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);       
        }
    }
}
