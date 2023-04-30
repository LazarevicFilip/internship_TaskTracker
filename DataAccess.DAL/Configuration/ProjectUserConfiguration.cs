using DataAccess.DAL.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core;

namespace DataAccess.DAL.Configuration
{
   
    internal class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUsers>
    {
        public void Configure(EntityTypeBuilder<ProjectUsers> builder)
        {
            builder.HasMany(x => x.Tasks).
                WithOne(x => x.ProjectUsers).
                HasForeignKey(x => x.ProjectUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        
    }
}
