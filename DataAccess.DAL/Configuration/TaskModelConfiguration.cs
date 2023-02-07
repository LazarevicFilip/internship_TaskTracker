using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DAL.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.DAL.Configuration
{
    internal class TaskModelConfiguration : EntityTypeConfiguration<TaskModel>
    {
        protected override void BaseConfiguring(EntityTypeBuilder<TaskModel> builder)
        {
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.Priority).IsRequired(false).HasDefaultValue(Priority.Low);
            //builder.Property(x => x.Status).HasDefaultValue(0);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
