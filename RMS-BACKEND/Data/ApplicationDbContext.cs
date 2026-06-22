using Microsoft.EntityFrameworkCore;
using RMS_BACKEND.Models;

namespace RMS_BACKEND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeLevel> EmployeeLevels { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee Configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasOne(e => e.EmployeeLevel)
                    .WithMany(el => el.Employees)
                    .HasForeignKey(e => e.EmployeeLevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Manager)
                    .WithMany(m => m.Subordinates)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EmployeeLevel Configuration
            modelBuilder.Entity<EmployeeLevel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            // Status Configuration
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
            });

            // TransactionType Configuration
            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedNever();
            });

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedNever();

                entity.HasOne(t => t.Employee)
                    .WithMany(e => e.Transactions)
                    .HasForeignKey(t => t.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.SubstituteEmployee)
                    .WithMany(e => e.SubstituteTransactions)
                    .HasForeignKey(t => t.SubstituteEmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.TransactionType)
                    .WithMany(tt => tt.Transactions)
                    .HasForeignKey(t => t.TransactionTypesID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Status)
                    .WithMany(s => s.Transactions)
                    .HasForeignKey(t => t.StatusID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
