using Microsoft.EntityFrameworkCore;
using Scheduler.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Data
{
    public class SchedulerContext : DbContext
    {
        public SchedulerContext(DbContextOptions<SchedulerContext> options)
            : base(options)
        {
        }

        public DbSet<JobDefinition> JobDefinitions { get; set; }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Modified = DateTime.Now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<SoftDeleteEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Unchanged;
                    entry.Entity.IsDeleted = false;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Modified = DateTime.Now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<SoftDeleteEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Unchanged;
                    entry.Entity.IsDeleted = true;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<JobDefinition>().Property(p => p.Name).HasMaxLength(30).IsRequired();
            builder.Entity<JobDefinition>().Property(p => p.Description).HasMaxLength(250).IsRequired();
            builder.Entity<JobDefinition>().Property(p => p.AssemblyName).HasMaxLength(200).IsRequired();
            builder.Entity<JobDefinition>().Property(p => p.MethodName).HasMaxLength(100).IsRequired();
            builder.Entity<JobDefinition>().Property(p => p.CronExpression).HasMaxLength(50).IsRequired();
            builder.Entity<JobDefinition>().Property(p => p.Modified).HasColumnType("datetime2");
            builder.Entity<JobDefinition>().Property(p => p.Created).HasColumnType("datetime2").HasDefaultValueSql("getdate()");

            builder.Entity<JobDefinition>().HasIndex(x => x.Name).IsUnique();

            base.OnModelCreating(builder);
        }
    }
}
