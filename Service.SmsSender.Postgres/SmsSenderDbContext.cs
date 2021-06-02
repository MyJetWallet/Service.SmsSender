using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service.SmsSender.Postgres
{
    public class SmsSenderDbContext : DbContext
    {
        public const string Schema = "smssender";

        public const string SentHistoryTableName = "sent_history";

        public DbSet<SentHistoryEntity> SentHistory { get; set; }

        public static ILoggerFactory LoggerFactory { get; set; }

        public SmsSenderDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<SentHistoryEntity>().ToTable(SentHistoryTableName);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<SentHistoryEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<SentHistoryEntity>().HasIndex(e => e.Id).IsUnique();
            
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.MaskedPhone).HasMaxLength(32);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.Template).HasMaxLength(64);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.Provider).HasMaxLength(64);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.ProcDate);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.ProcError).HasMaxLength(128);
            modelBuilder.Entity<SentHistoryEntity>().Property(e => e.ClientId).HasMaxLength(64);

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsetAsync(IEnumerable<SentHistoryEntity> entities)
        {
            var result = await SentHistory.UpsertRange(entities).On(e => e.Id).NoUpdate().RunAsync();
            return result;
        }
    }
}

