using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Postgres
{
    public class SmsSenderDbContext : DbContext
    {
        public const string Schema = "smssender";

        public const string SentHistoryTableName = "sent_history";

        public DbSet<SentHistoryRecord> SentHistory { get; set; }

        public static ILoggerFactory LoggerFactory { get; set; }

        public SmsSenderDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<SentHistoryRecord>().ToTable(SentHistoryTableName);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<SentHistoryRecord>().HasKey(e => e.Id);
            
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.MaskedPhone).HasMaxLength(32);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.Brand).HasMaxLength(64);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.Template).HasMaxLength(64);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.Provider).HasMaxLength(64);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.ProcDate);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.ProcError).HasMaxLength(1280);
            modelBuilder.Entity<SentHistoryRecord>().Property(e => e.ClientId).HasMaxLength(64);

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsetAsync(IEnumerable<SentHistoryRecord> entities)
        {
            var result = await SentHistory.UpsertRange(entities).On(e => e.Id).NoUpdate().RunAsync();
            return result;
        }
    }
}

