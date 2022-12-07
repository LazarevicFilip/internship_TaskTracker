using Domain.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Configuration
{
    internal class UserConfiguration : EntityTypeConfiguration<User>
    {
        protected override void BaseConfiguring(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.UserName).IsRequired(true).HasMaxLength(20);
            builder.Property(x => x.FirstName).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.Password).IsRequired(true).HasMaxLength(120);
            builder.Property(x => x.Email).IsRequired(true).HasMaxLength(40);

            builder.HasIndex(x => x.Email).IsUnique(true);
            builder.HasIndex(x => x.UserName).IsUnique(true);
            builder.HasIndex(x => x.LastName);
            builder.HasIndex(x => x.FirstName);
        }
    }
}
