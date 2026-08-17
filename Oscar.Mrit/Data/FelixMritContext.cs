using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Oscar.Mrit.Data
{
    public class FelixMritContext: DbContext
    {
        public FelixMritContext(DbContextOptions<FelixMritContext> options): base(options)
        {
        }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Transmission> Transmissions { get; set; }
        public DbSet<Works> Works { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonOfInterest> PersonsOfInterest { get; set; }
        public DbSet<PersonType> PersonTypes { get; set; }
        public DbSet<AltProductionTitle> AltProductionTitle { get; set; }
        public DbSet<AltRecordTitle> AltRecordTitles { get; set; }
        public DbSet<Territory> Territories { get; set; }
        public DbSet<BatchJob> BatchJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PersonType>().HasData(
                new PersonType {Id = 1, CreateDate = DateTime.Now, Name = "Director"},
                new PersonType { Id = 2, CreateDate = DateTime.Now, Name = "Actor" },
                new PersonType { Id = 3, CreateDate = DateTime.Now, Name = "Producer" },
                new PersonType { Id = 4, CreateDate = DateTime.Now, Name = "Creator" },
                new PersonType { Id = 5, CreateDate = DateTime.Now, Name = "Writer" }
                );

            modelBuilder.Entity<Match>()
                .HasOne(m => m.BatchJob)
                .WithMany(x => x.Matches)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transmission>()
                .HasOne(m => m.Match)
                .WithMany(x => x.Transmissions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transmission>()
                .HasIndex(t => t.MritId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added));

            foreach (var entityEntry in entries)
                ((BaseEntity)entityEntry.Entity).CreateDate = DateTime.Now;

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
