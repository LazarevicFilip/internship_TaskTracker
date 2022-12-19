﻿using DataAccess.DAL.Core;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL
{
    public class TaskContext : DbContext
    {
        private readonly IConfiguration _settings;
        public TaskContext(DbContextOptions<TaskContext> options, IConfiguration settings)
            : base(options)
        {
            _settings = settings;
        }
        public TaskContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=localhost;Initial Catalog=task_tracker;Integrated Security=True");
            optionsBuilder
                
                 .UseSqlServer(_settings.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<ProjectModel>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<TaskModel>().HasQueryFilter(x => x.IsActive);
            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity e)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            e.CreatedAt = DateTime.Now;
                            e.IsActive = true;
                            break;
                        case EntityState.Modified:
                            e.UpdatedAt = DateTime.Now;
                            break;
                    }
                }
            }
            return base.SaveChanges();
        }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
