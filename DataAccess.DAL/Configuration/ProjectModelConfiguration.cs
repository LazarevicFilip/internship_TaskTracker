using DataAccess.DAL.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Configuration
{
    internal class ProjectModelConfiguration : EntityTypeConfiguration<ProjectModel>
    {
        protected override void BaseConfiguring(EntityTypeBuilder<ProjectModel> builder)
        {
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(60);
            builder.HasIndex(x => x.Name).IsUnique(true);

            builder.HasMany(x => x.Tasks)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new ProjectModel
            {
                Id = 1,
                Name = "Project from seeder.",
                CreatedAt= DateTime.UtcNow,
                StartDate= DateTime.UtcNow,
                CompletionDate = DateTime.UtcNow.AddMonths(4),
                ProjectStatus = ProjectStatus.NotStarted,
                ProjectPriority = Priority.VeryHigh
            });
        }
    }
}
