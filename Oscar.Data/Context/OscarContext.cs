using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;
using EntityFramework.Exceptions.SqlServer;
using Oscar.Core.Enums;
using Oscar.Core.Providers;
using WorksStatus = Oscar.Core.Entities.WorksStatus;

namespace Oscar.Data.Context
{
    public sealed class OscarContext: DbContext
    {
        private readonly IUserProvider _userProvider;

        public OscarContext()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public OscarContext(DbContextOptions<OscarContext> options, IUserProvider userProvider)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            if (!Database.IsInMemory())
            {
                Database.SetCommandTimeout(120 * 60);
            }

            _userProvider = userProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseExceptionProcessor();
        }

        public DbSet<WorksTitleResult> WorksTitleResults { get; set; }

        public DbSet<MerlinSociety> MerlinSocieties { get; set; }
        public DbSet<ClientCatalogueSocietyWork> ClientCatalogueSocietyWorks { get; set; }
        public DbSet<WorksTitle> WorksTitles { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Works> Works { get; set; }
        public DbSet<StandAlone> StandAlones { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GenreSubType> GenreSubTypes { get; set; }
        public DbSet<WorksSubType> WorksSubTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<WorksStatus> WorksStatuses { get; set; }
        public DbSet<Core.Entities.RegistrationConfiguration> RegistrationConfigurations { get; set; }
        
        public DbSet<CustomerServiceManager> CustomServiceManagers { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<MatchRequest> MatchRequests { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<WorksImportRequest> WorksImportRequests { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportField> ReportFields { get; set; }
        public DbSet<ReportEntityJoin> ReportentityJoins { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<ScreenWriter> ScreenWriters { get; set; }
        public DbSet<ScriptWriter> ScriptWriters { get; set; }
        public DbSet<WorksImport> WorksImports { get; set; }
        public DbSet<WorksRightsImport> WorksRightsImports { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<WorksType> WorksTypes { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<RightsType> RightsTypes { get; set; }
        public DbSet<LanguageRights> LanguageRights { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }
        public DbSet<SocietyReference> SocietyReferences { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<RegistrationBatch> RegistrationBatches { get; set; }
        public DbSet<Society> Societies { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<CountryGroup> CountryGroup { get; set; }
        public DbSet<WorksHeader> WorksHeaders { get; set; }
        public DbSet<Mandate> Mandates { get; set; }
        public DbSet<MandateType> MandateType { get; set; }
        public DbSet<EquivalenceRequest> EquivalenceRequests { get; set; }
        public DbSet<ScreenrightsRequest> ScreenrightsRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ClientAltName> ClientAltNames { get; set; }
        public DbSet<ChannelRights> ChannelRights { get; set; }
        public DbSet<Core.Entities.ReRegistration> ReRegistrations { get; set; }
        public DbSet<OtherName> OtherName { get; set; }
        public DbSet<WorksStatusHistory> WorksStatusHistory { get; set; }
        public DbSet<VwOnMusicFelixWorks> VwOnMusicFelixWorks { get; set; }
        public DbSet<OnMusicMatch> OnMusicMatches { get; set; }
        public DbSet<OnMusicMatchStatus> OnMusicMatchStatuses { get; set; }

        public DbSet<ClientDetail> ClientDetails { get; set; }
        public DbSet<ClientCataloguesDetail> ClientCataloguesDetails { get; set; }
        public DbSet<ClientWorkItem> ClientWorkItems { get; set; }
        public DbSet<ProductionCountryItem> ProductionCountryItems { get; set; }
        public DbSet<ClientWorkRightItem> ClientWorkRightItems { get; set; }
        public DbSet<ClientWorkStatItem> ClientWorkYearlyStats { get; set; }
        public DbSet<ClientWorkStatItemEx> ClientWorkProductionYearlyStats { get; set; }
        public DbSet<WorksDetails> WorksDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VwOnMusicFelixWorks>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_OnMusic_Felix_Works");

                entity.Property(e => e.Actors)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.As400refNo)
                    .IsRequired()
                    .HasColumnName("AS400RefNo")
                    .HasMaxLength(20);

                entity.Property(e => e.CatalogueName).HasMaxLength(75);

                entity.Property(e => e.ClientGrade)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.ClientName).HasMaxLength(50);

                entity.Property(e => e.Genre).HasMaxLength(2);

                entity.Property(e => e.GenreSubType).HasMaxLength(3);

                //entity.Property(e => e.ClientStatus).HasConversion<Status>();

                entity.Property(e => e.CompactRef).HasMaxLength(15);

                entity.Property(e => e.ContractEndDate)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Directors)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Duration).HasMaxLength(30);

                entity.Property(e => e.FirstBroadcastYear).HasMaxLength(4);

                entity.Property(e => e.Isan)
                    .HasColumnName("ISAN")
                    .HasMaxLength(26);

                entity.Property(e => e.Producers)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ProductionCompanies)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ProductionYear).HasMaxLength(4);

                entity.Property(e => e.SeasonRef).HasMaxLength(15);

                entity.Property(e => e.SeriesRef).HasMaxLength(15);

                entity.Property(e => e.SeriesTitle)
                    .HasColumnName("Series_Title")
                    .HasMaxLength(255);

                entity.Property(e => e.Titles).HasMaxLength(4000);

                entity.Property(e => e.WorkType).HasMaxLength(2);

                entity.Property(e => e.WorksReference).HasMaxLength(50);
            });

            modelBuilder.Entity<OnMusicMatchStatus>().HasData(new OnMusicMatchStatus() { Id = 1, Name = "Success"});
            modelBuilder.Entity<OnMusicMatchStatus>().HasData(new OnMusicMatchStatus { Id = 2, Name = "Error" });
            modelBuilder.Entity<OnMusicMatchStatus>().HasData(new OnMusicMatchStatus { Id = 3, Name = "Duplicate" });

            modelBuilder.Entity<ClientCatalogueSocietyWork>(entity =>
                entity.HasKey(e => e.WorksId));

            modelBuilder
                .Entity<WorksHeader>()
                .ToView("vw_SearchTitles")
                .HasKey(t => t.WorksId);

            modelBuilder.Entity<Country>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder
                .Entity<Language>()
                .HasMany(e => e.LanguageRights)
                .WithOne(e => e.Language);

            modelBuilder
                .Entity<Works>()
                .HasOne(e => e.WorksImportRequest)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Series>()
                .HasMany(e => e.Seasons)
                .WithOne(e => e.Series)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<StandAlone>()
                .HasOne(e => e.Genre)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Episode>()
                .HasOne(e => e.Season)
                .WithMany(e => e.Episodes)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Season>()
                .HasOne(e => e.Series)
                .WithMany(e => e.Seasons)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<WorksStatusHistory>()
                .HasOne(e => e.OldStatus)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<WorksStatusHistory>()
                .HasOne(e => e.NewStatus)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<WorksImportRequest>()
                .HasOne(e => e.Client);

            //.WithMany(e => e.WorksImportRequests);

            modelBuilder
                .Entity<WorksImportRequest>()
                .HasOne(e => e.Catalogue);
            //    .WithMany(e => e.WorksImportRequests)
            //    .HasForeignKey(e => e.CatalogueId);

            modelBuilder
                .Entity<Contract>()
                .HasOne(c => c.Client)
                .WithOne(c => c.Contract)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<WorksTitle>().ToTable("WorksTitle", b => b.IsTemporal());

            modelBuilder.Entity<Actor>().ToTable("Actor", b => b.IsTemporal());

            modelBuilder.Entity<Genre>().ToTable("Genre");

            modelBuilder.Entity<GenreSubType>().ToTable("GenreSubType");

            modelBuilder.Entity<WorksSubType>().ToTable("WorksSubType");

            modelBuilder.Entity<Right>().ToTable("Rights", b => b.IsTemporal());

            modelBuilder.Entity<RightsType>().ToTable("RightsType");

            modelBuilder.Entity<Contact>().ToTable("Contact", b => b.IsTemporal());

            modelBuilder.Entity<WorksStatus>().ToTable("WorksStatus");

            modelBuilder.Entity<Core.Entities.RegistrationConfiguration>().ToTable("RegistrationConfiguration");

            modelBuilder.Entity<CustomerServiceManager>().ToTable("CustomerServiceManager");

            modelBuilder.Entity<WorksImportRequest>().ToTable("WorksImportRequest");
            modelBuilder.Entity<MerlinSociety>().ToTable("MerlinSociety");

            modelBuilder
                .Entity<WorksImport>()
                .ToTable("WorksImport")
                .HasOne(e => e.WorksImportRequest)
                .WithMany(e => e.WorksImports);

            modelBuilder
                .Entity<WorksRightsImport>()
                .ToTable("WorksRightsImport")
                .HasOne(e => e.WorksImport)
                .WithMany(e => e.WorksRightsImports);

            modelBuilder
                .Entity<MatchRequest>()
                .ToTable("MatchRequests");

            modelBuilder
                .Entity<Company>()
                .ToTable("Company");

            modelBuilder.Entity<Works>().ToTable("Works", b => b.IsTemporal());
            modelBuilder.Entity<Client>().ToTable("Clients", b => b.IsTemporal());
            modelBuilder.Entity<WorksType>().ToTable("WorksType");
            modelBuilder.Entity<Address>().ToTable("Address", b => b.IsTemporal());
            modelBuilder.Entity<Contract>().ToTable("Contract");
            modelBuilder.Entity<Language>().ToTable("Language");
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Country>().Property(e => e.Name).HasMaxLength(50);
            modelBuilder.Entity<Country>().Property(e => e.Code).HasMaxLength(2);
            modelBuilder.Entity<Country>().Property(e => e.Code3A).HasMaxLength(3);
            modelBuilder.Entity<Report>().ToTable("Report");
            modelBuilder.Entity<ReportField>().ToTable("ReportField");
            modelBuilder.Entity<ReportEntityJoin>().ToTable("ReportEntityJoin");
            modelBuilder.Entity<Catalogue>().ToTable("Catalogue");
            modelBuilder.Entity<Producer>().ToTable("Producer", b => b.IsTemporal());
            modelBuilder.Entity<Director>().ToTable("Director", b => b.IsTemporal());
            modelBuilder.Entity<Distributor>().ToTable("Distributor", b => b.IsTemporal());
            modelBuilder.Entity<ScreenWriter>().ToTable("ScreenWriter", b => b.IsTemporal());
            modelBuilder.Entity<ScriptWriter>().ToTable("ScriptWriter", b => b.IsTemporal());
            modelBuilder.Entity<Conflict>().ToTable("Conflict", b => b.IsTemporal());
            modelBuilder.Entity<Registration>().ToTable("Registration");
            modelBuilder.Entity<RegistrationBatch>().ToTable("RegistrationBatch");
            modelBuilder.Entity<Society>().ToTable("Society", b => b.IsTemporal());
            modelBuilder.Entity<OtherName>().ToTable("OtherName");


            modelBuilder.Entity<Core.Entities.ReRegistration>().ToTable("ReRegistration");
            modelBuilder.Entity<Core.Entities.SocietyReference>().ToTable("SocietyReference");

            modelBuilder.Entity<OtherName>().ToTable("OtherName");


            modelBuilder.Entity<Works>()
                .Property("Discriminator")
                .IsUnicode(false)
                .HasMaxLength(10);

            modelBuilder.Entity<Works>()
                .HasIndex("Discriminator");

            modelBuilder.Entity<WorksTitle>()
                .Property("Title")
                .IsUnicode(false)
                .HasMaxLength(1000);

            modelBuilder.Entity<WorksTitle>()
                .HasIndex("Title");

            modelBuilder.Entity<WorksTitle>()
                .Property("ReverseTitle")
                .IsUnicode(false)
                .HasMaxLength(1000);

            modelBuilder.Entity<WorksTitle>()
                .HasIndex("ReverseTitle");

            modelBuilder.Entity<Actor>()
                .Property("FirstName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<Actor>()
                .Property("LastName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<Director>()
                .Property("FirstName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<Director>()
                .Property("LastName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<Producer>()
                .Property("FirstName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<Producer>()
                .Property("LastName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<ScreenWriter>()
                .Property("FirstName")
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<ScreenWriter>()
                .Property("LastName")
                .IsUnicode(false)
                .HasMaxLength(500);

            var createDate = new DateTime(2023,1,1);

            //modelBuilder.Entity<ChannelRights>()
            //    .HasOne<Right>()
            //    .WithMany(r => r.ChannelRights)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<LanguageRights>()
            //    .HasOne<Right>()
            //    .WithMany(r => r.LanguageRights)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 1, Name = "CN", Description = "Cartoon", CreationDate = createDate, ModifiedBy = "SEED"});
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 2, Name = "DO", Description = "Documentary", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 3, Name = "GS", Description = "Game Show", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 4, Name = "MG", Description = "Magazine", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 5, Name = "MC", Description = "Music Concert", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 6, Name = "OB", Description = "Opera/Ballet", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 7, Name = "SD", Description = "Short Documentary", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 8, Name = "SK", Description = "Sketch", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 9, Name = "TH", Description = "Theatre", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 10, Name = "VS", Description = "Variety Show", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 11, Name = "DR", Description = "Drama", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksSubType>().HasData(new WorksSubType { Id = 12, Name = "EN", Description = "Entertainment", CreationDate = createDate, ModifiedBy = "SEED" });

            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = -1, Name = "Any", Description = "Any", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 1, Name = "Active", Description = "Active", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 2, Name = "Uncontrolled", Description = "Uncontrolled", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 3, Name = "Incomplete", Description = "Incomplete", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 4, Name = "Relinquished", Description = "Relinquished", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 5, Name = "InConflict", Description = "InConflict", CreationDate = createDate, ModifiedBy = "SEED" });
            modelBuilder.Entity<WorksStatus>().HasData(new WorksStatus { Id = 6, Name = "Duplicate", Description = "Duplicate", CreationDate = createDate, ModifiedBy = "SEED" });

            //To show registrations list in Oscar only after this go live Oscar date - to filter out Felix registrations
            string defaultRegistrationDate = "2024-01-31 23:00:00"; 
            modelBuilder.Entity<Core.Entities.RegistrationConfiguration>().HasData(new Core.Entities.RegistrationConfiguration { Id = 1, Name = "RegistrationBatch", Description = "Registration Batch", RegistrationDate = Convert.ToDateTime(defaultRegistrationDate) , CreationDate = createDate, ModifiedBy = "SEED" });

            //modelBuilder.Entity<Client>()
            //    .HasMany(e => e.CustomServiceManagers)
            //    .WithOne(e => e.Client);
            ////.UsingEntity<ClientCustomServiceManager>();

            //modelBuilder.Entity<CustomServiceManager>()
            //    .HasMany(e => e.Clients)
            //    .WithOne(e => e.CustomServiceManager);

            //modelBuilder.Entity<ClientCustomServiceManager>()
            //    .HasNoKey();

            //modelBuilder.Entity<WorksTitleResult>().HasNoKey().ToView(null);

            //Telling EF to ignore creating below dbset as table as it's used only to get results back from stored proc call
            modelBuilder.Entity<WorksTitleResult>().ToTable(nameof(WorksTitleResult), t => t.ExcludeFromMigrations());
            modelBuilder.Entity<WorksTitleResult>(entity => entity.HasNoKey());

            ReportModelBuild(modelBuilder);
        }

        private void ReportModelBuild(ModelBuilder builder)
        {
            // Client Detail
            builder.Entity<ClientDetail>(e =>
            {
                e.HasNoKey();
                e.ToView("V_ClientDetails");
                e.Property(p => p.ClientCreatedOn).HasColumnType("datetime2");
                e.Property(p => p.ClientStartOn).HasColumnType("datetime2");
                e.Property(p => p.ClientEndOn).HasColumnType("datetime2");
                e.Property(p => p.ContractFirstStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContractCurrentStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContractEndDate).HasColumnType("datetime2");
                e.Property(p => p.ContactStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContactEndDate).HasColumnType("datetime2");
                e.Property(p => p.ContactCreationDate).HasColumnType("datetime2");
            });

            // Client Catalogues Detail
            builder.Entity<ClientCataloguesDetail>(e =>
            {
                e.HasNoKey();
                e.ToView("V_ClientCataloguesDetails");
                e.Property(p => p.ClientCreatedOn).HasColumnType("datetime2");
                e.Property(p => p.ClientStartOn).HasColumnType("datetime2");
                e.Property(p => p.ClientEndOn).HasColumnType("datetime2");
                e.Property(p => p.ContractFirstStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContractCurrentStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContractEndDate).HasColumnType("datetime2");
                e.Property(p => p.ContactStartDate).HasColumnType("datetime2");
                e.Property(p => p.ContactEndDate).HasColumnType("datetime2");
                e.Property(p => p.ContactCreationDate).HasColumnType("datetime2");
            });

            // Client Works Item
            builder.Entity<ClientWorkItem>(e =>
                {
                    e.HasKey(x => x.WorksId);
                    e.ToView("V_ClientWorksList");
                    e.Property(p => p.CreationDate).HasColumnType("datetime2");
                    e.Property(p => p.LastModified).HasColumnType("datetime2");
                });

            // Production Country Item
            builder.Entity<ProductionCountryItem>(e =>
            {
                e.HasKey(x => x.WorksId);
                e.ToView("V_ClientProductionCountries");
            });

            // Client Work List Of Rights
            builder.Entity<ClientWorkRightItem>(e =>
            {
                e.HasNoKey();
                e.ToView("V_ClientWorkListOfRights");
                e.Property(p => p.StartDate).HasColumnType("datetime2");
                e.Property(p => p.EndDate).HasColumnType("datetime2");
                e.Property(p => p.Percentage).HasPrecision(18, 2);
            });

            // Client Registration KPI Report
            builder.Entity<ClientWorkStatItem>(e => { e.HasNoKey(); e.ToView("V_ClientWorkYearlyStats"); });

            builder.Entity<ClientWorkStatItemEx>(e => { e.HasNoKey(); e.ToView("V_ClientWorkStatsByProductionYear"); });

            // Works Detail
            builder.Entity<WorksDetails>(e =>
            {
                e.HasKey(x => x.WorksId);
                e.ToView("V_WorksDetails");
                e.Property(p => p.CreationDate).HasColumnType("datetime2");
            });
        }

        public override int SaveChanges()
        {
            SetAuditDetails();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditDetails();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditDetails()
        {
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries();
            var added = entries.Where(t => t.State == EntityState.Added)
                        .Select(t => t.Entity)
                        .ToList();
            var modified = entries.Where(t => t.State == EntityState.Modified)
                        .Select(t => t.Entity)
                        .ToList();


            foreach (var add in added)
            {
                if (add is BaseEntity entity)
                {
                    entity.CreationDate = DateTime.UtcNow;
                    entity.LastModified = DateTime.UtcNow;
                    entity.ModifiedBy = _userProvider.GetUserName() ?? "System";
                }
            }

            foreach (var mod in modified)
            {
                if (mod is BaseEntity entity)
                {
                    entity.LastModified = DateTime.UtcNow;
                    entity.ModifiedBy = _userProvider.GetUserName() ?? "System";
                }

            }
        }
    }

}
