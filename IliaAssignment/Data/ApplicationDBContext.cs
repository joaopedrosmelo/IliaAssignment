using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Data
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<CustomerDB> CustomerDBs { get; set; }
        public DbSet<OrdersDB> OrdersDBs { get; set; }
        public DbSet<OrderStatusDB> OrderStatusDBs { get; set; }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdersDB>()
                .HasOne(c => c.CustomerDB)
                .WithMany(o => o.OrdersDB)
                .HasForeignKey(c => c.IdCustomer);

            modelBuilder.Entity<OrdersDB>()
                .HasOne(s => s.OrderStatusDB)
                .WithOne(o => o.OrdersDB)
                .HasForeignKey<OrdersDB>(s => s.IdStatus);
        }
    }
}
