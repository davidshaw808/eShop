﻿using Common;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Databases.Base
{
    public class CrispHabitatBaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<HistoryLog> HistoryLogs { get; set; }
        public DbSet<OrderUpdate> OrderUpdates { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
               .HasIndex(a => a.AltId);
            modelBuilder.Entity<Customer>()
              .HasIndex(a => a.Email);
            modelBuilder.Entity<Refund>()
               .HasIndex(a => a.AltId);

            modelBuilder.Entity<Category>()
               .HasOne(c => c.Parent)
               .WithMany()
               .HasForeignKey(a => a.ParentId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne();

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithMany();
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.OrderHistory)
                .WithOne(o => o.Customer);
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Basket)
                .WithOne();
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithMany()
                .UsingEntity("OrderProducts");
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithOne()
                .HasForeignKey<Order>("AddressId");
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.OrderHistory);
            modelBuilder.Entity<Order>()
               .HasMany(o => o.Updates)
               .WithOne();
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Refunds)
                .WithMany(r => r.Orders)
                .UsingEntity("OrderRefunds");

            modelBuilder.Entity<Order>()
               .HasOne(o => o.PaymentDetails)
               .WithOne()
               .HasForeignKey<Order>("PaymentId");

        }
    }
}
