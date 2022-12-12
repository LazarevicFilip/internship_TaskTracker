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
    internal abstract class EntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            BaseConfiguring(builder);
        }
        //template method pattern
        protected abstract void BaseConfiguring(EntityTypeBuilder<T> builder);
        
    }
}
