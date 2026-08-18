/*
    SeedDummyData.sql
    -----------------
    Loads a development database with dummy Clients, Societies and Works.

    Works cover all four discriminators, with a real hierarchy:
        Series  -> Seasons -> Episodes        (episodes carry SeasonId)
        Series  -> Episodes                   (episodes carry Episode_SeriesId, no season)
        StandAlone

    FOR DEVELOPMENT AND TEST DATABASES ONLY.

    The script refuses to run unless @ExpectedDatabase matches the database it is
    executed against, so it cannot be fired at the wrong target by accident.

    Re-runnable: every row it creates is stamped with ModifiedBy = @SeedTag, and the
    script deletes rows carrying that tag before inserting. It will not touch data
    it did not create.

    Usage:
        sqlcmd -S localhost -d OscarUiDev -E -i SeedDummyData.sql
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* Required: the database carries indexed views, so writes fail without these.
   sqlcmd defaults QUOTED_IDENTIFIER to OFF, unlike SSMS. */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @ExpectedDatabase sysname = N'OscarUiDev';   -- change to match your dev database
DECLARE @SeedTag          nvarchar(50) = N'seed-dummy-data';

/* Volume knobs. Episode counts are per season. */
DECLARE @SocietyCount        int = 8;
DECLARE @ClientCount         int = 12;
DECLARE @StandaloneCount     int = 40;
DECLARE @SeriesCount         int = 10;
DECLARE @SeasonsPerSeries    int = 3;
DECLARE @EpisodesPerSeason   int = 8;
DECLARE @LooseEpisodes       int = 4;   -- episodes hung directly off a series, with no season

------------------------------------------------------------------------------
-- 0. Guard
------------------------------------------------------------------------------
DECLARE @ActualDatabase sysname = DB_NAME();

IF @ActualDatabase <> @ExpectedDatabase
BEGIN
    RAISERROR(
        'Refusing to seed: connected to database "%s" but @ExpectedDatabase is "%s". Edit @ExpectedDatabase if this really is your dev database.',
        16, 1, @ActualDatabase, @ExpectedDatabase);
    RETURN;
END

PRINT CONCAT('Seeding ', DB_NAME(), ' on ', @@SERVERNAME, ' with tag "', @SeedTag, '"');

BEGIN TRY
BEGIN TRANSACTION;

------------------------------------------------------------------------------
-- 1. Remove anything a previous run of this script created
------------------------------------------------------------------------------
DECLARE @SeededWorks TABLE (Id int PRIMARY KEY);
INSERT INTO @SeededWorks (Id) SELECT Id FROM dbo.Works WHERE ModifiedBy = @SeedTag;

DECLARE @SeededClients TABLE (Id int PRIMARY KEY);
INSERT INTO @SeededClients (Id) SELECT Id FROM dbo.Clients WHERE ModifiedBy = @SeedTag;

DELETE FROM dbo.CatalogueWorks WHERE WorksId IN (SELECT Id FROM @SeededWorks);
DELETE FROM dbo.ClientWorks    WHERE WorksId IN (SELECT Id FROM @SeededWorks);
DELETE FROM dbo.WorksTitle     WHERE WorksId IN (SELECT Id FROM @SeededWorks);

/* Children before parents: episodes, then seasons, then series/standalone. */
DELETE FROM dbo.Works WHERE Id IN (SELECT Id FROM @SeededWorks) AND Discriminator = 'Episode';
DELETE FROM dbo.Works WHERE Id IN (SELECT Id FROM @SeededWorks) AND Discriminator = 'Season';
DELETE FROM dbo.Works WHERE Id IN (SELECT Id FROM @SeededWorks);

DELETE FROM dbo.ClientSociety WHERE ClientsId IN (SELECT Id FROM @SeededClients);
DELETE FROM dbo.Catalogue     WHERE ClientId  IN (SELECT Id FROM @SeededClients);
DELETE FROM dbo.Clients       WHERE Id        IN (SELECT Id FROM @SeededClients);
DELETE FROM dbo.Society       WHERE ModifiedBy = @SeedTag;

/* Lookup rows this script added, but only where nothing still points at them.
   Keeps a re-run from accumulating stale entries if the lists below change,
   while never removing a row real data depends on. */
DELETE FROM dbo.WorksType
WHERE ModifiedBy = @SeedTag
  AND NOT EXISTS (SELECT 1 FROM dbo.Works w WHERE w.WorksTypeId = dbo.WorksType.Id);

DELETE FROM dbo.Genre
WHERE ModifiedBy = @SeedTag
  AND NOT EXISTS (SELECT 1 FROM dbo.Works w WHERE w.GenreId = dbo.Genre.Id);

------------------------------------------------------------------------------
-- 2. Lookups (only inserted when missing, matched by name)
------------------------------------------------------------------------------
/* Name is a short code and Description the label, matching the WorksSubType rows
   seeded by migration 20231128170642_seed-work-subtypes.

   These codes are not optional dressing: SeriesForm.razor renders
   <WorksTypePicker DefaultValue="SE">, and WorksTypePicker.OnInitializedAsync does
   _types.FirstOrDefault(x => x.Name == DefaultValue).Id — which throws a
   NullReferenceException if no WorksType is named exactly 'SE'. */
DECLARE @WorksTypes TABLE (Name nvarchar(200), Description nvarchar(400));
INSERT INTO @WorksTypes VALUES
    (N'SH', N'Short Film'),
    (N'SE', N'Series or Serials'),
    (N'FF', N'Feature Film'),
    (N'TF', N'TV Film'),
    (N'MC', N'Music Concert'),
    (N'OB', N'Opera/Ballet'),
    (N'VS', N'Variety Show'),
    (N'SK', N'Sketch'),
    (N'GS', N'Game Show'),
    (N'TH', N'Theatre'),
    (N'CN', N'Cartoon'),
    (N'DO', N'Documentary'),
    (N'SD', N'Short Documentary'),
    (N'MG', N'Magazine');

INSERT INTO dbo.WorksType (Name, Description, CreationDate, ModifiedBy)
SELECT t.Name, t.Description, SYSUTCDATETIME(), @SeedTag
FROM @WorksTypes t
WHERE NOT EXISTS (SELECT 1 FROM dbo.WorksType wt WHERE wt.Name = t.Name);

DECLARE @Genres TABLE (Name nvarchar(200), Description nvarchar(400));
INSERT INTO @Genres VALUES
    (N'FI', N'Fiction'),
    (N'AN', N'Animation'),
    (N'NF', N'Non-Fiction'),
    (N'ZZ', N'Unknown');

INSERT INTO dbo.Genre (Name, Description, CreationDate, ModifiedBy)
SELECT g.Name, g.Description, SYSUTCDATETIME(), @SeedTag
FROM @Genres g
WHERE NOT EXISTS (SELECT 1 FROM dbo.Genre gn WHERE gn.Name = g.Name);

/* Resolve lookup ids into ordered, 1-based lists so works can round-robin over them. */
/* Standalone works rotate over the non-series types; series, seasons and episodes
   all take 'SE', which is what SeriesForm pins the picker to. */
DECLARE @TypeIds TABLE (Rn int, Id int);
INSERT INTO @TypeIds (Rn, Id)
SELECT ROW_NUMBER() OVER (ORDER BY Id), Id FROM dbo.WorksType WHERE Name <> N'SE';

DECLARE @SeriesTypeId int = (SELECT TOP 1 Id FROM dbo.WorksType WHERE Name = N'SE');

IF @SeriesTypeId IS NULL
    RAISERROR('WorksType ''SE'' is missing; SeriesForm would throw on load.', 16, 1);

DECLARE @GenreIds TABLE (Rn int, Id int);
INSERT INTO @GenreIds (Rn, Id)
SELECT ROW_NUMBER() OVER (ORDER BY Id), Id FROM dbo.Genre;

DECLARE @SubTypeIds TABLE (Rn int, Id int);
INSERT INTO @SubTypeIds (Rn, Id)
SELECT ROW_NUMBER() OVER (ORDER BY Id), Id FROM dbo.WorksSubType;

DECLARE @TypeCount    int = (SELECT COUNT(*) FROM @TypeIds);
DECLARE @GenreCount   int = (SELECT COUNT(*) FROM @GenreIds);
DECLARE @SubTypeCount int = (SELECT COUNT(*) FROM @SubTypeIds);

------------------------------------------------------------------------------
-- 3. Societies
------------------------------------------------------------------------------
DECLARE @SocietyNames TABLE (Rn int IDENTITY(1,1), Name nvarchar(200), Expanded nvarchar(400));
INSERT INTO @SocietyNames (Name, Expanded) VALUES
    (N'AGICOA',      N'Association of International Collective Management of Audiovisual Works'),
    (N'SCREENRIGHTS',N'Audio-Visual Copyright Society'),
    (N'SUISSIMAGE',  N'Swiss authors rights co-operative'),
    (N'EGEDA',       N'Entidad de Gestion de Derechos de los Productores Audiovisuales'),
    (N'GWFF',        N'Gesellschaft zur Wahrnehmung von Film- und Fernsehrechten'),
    (N'CCC',         N'Copyright Collective of Canada'),
    (N'MPA',         N'Motion Picture Association'),
    (N'MPLC',        N'Motion Picture Licensing Company'),
    (N'CRC',         N'Canadian Retransmission Collective'),
    (N'UPFAR ARGOA', N'Uniunea Producatorilor de Film si Audiovizual din Romania');

INSERT INTO dbo.Society (Name, ExpandedName, GeneralNotes, IsClientRegistration, IsWorksRegistration, Website, CreationDate, ModifiedBy)
SELECT s.Name,
       s.Expanded,
       CONCAT(N'Dummy society record for ', s.Name),
       CASE WHEN s.Rn % 3 = 0 THEN 1 ELSE 0 END,
       1,
       CONCAT(N'https://example.org/', LOWER(REPLACE(s.Name, N' ', N'-'))),
       SYSUTCDATETIME(),
       @SeedTag
FROM @SocietyNames s
WHERE s.Rn <= @SocietyCount
ORDER BY s.Rn;

------------------------------------------------------------------------------
-- 4. Clients and their catalogues
------------------------------------------------------------------------------
DECLARE @ClientNames TABLE (Rn int IDENTITY(1,1), Name nvarchar(200));
INSERT INTO @ClientNames (Name) VALUES
    (N'NORTHERN LIGHT PICTURES'), (N'HARBOUR ROAD MEDIA'),   (N'CEDAR & PINE PRODUCTIONS'),
    (N'BLUE WHARF TELEVISION'),   (N'SILVERGATE STUDIOS'),   (N'MERIDIAN FACTUAL'),
    (N'OLD MILL FILMS'),          (N'KESTREL BROADCAST'),    (N'LANTERN HOUSE MEDIA'),
    (N'SALTMARSH ENTERTAINMENT'), (N'IRONBRIDGE PICTURES'),  (N'FIVE ACRE FILMS'),
    (N'WESTCLIFF DISTRIBUTION'),  (N'GREENWOOD ANIMATION'),  (N'TIDEWAY DOCUMENTARIES');

INSERT INTO dbo.Clients
    (ClientName, ClientReference, Email, GeneralNotes, Status, ClientGrade, ClientType,
     AgicoaClientRef, IMaestroClientCode, CreationDate, ModifiedBy)
SELECT c.Name,
       100000 + c.Rn,
       CONCAT(N'rights@', LOWER(REPLACE(REPLACE(c.Name, N' ', N''), N'&', N'')), N'.example'),
       CONCAT(N'Dummy client record #', c.Rn),
       1,                                       -- Status: active
       ((c.Rn - 1) % 7) + 1,                    -- ClientGrade: Bronze..Anthem
       ((c.Rn - 1) % 3) + 1,                    -- ClientType: Broadcaster / Distributor / FilmTVProducer
       CONCAT(N'AG', RIGHT(N'00000' + CAST(c.Rn AS nvarchar(10)), 5)),
       CONCAT(N'IM', RIGHT(N'00000' + CAST(c.Rn AS nvarchar(10)), 5)),
       SYSUTCDATETIME(),
       @SeedTag
FROM @ClientNames c
WHERE c.Rn <= @ClientCount
ORDER BY c.Rn;

/* Ordered client list for downstream round-robin assignment. */
DECLARE @Clients TABLE (Rn int, Id int, ClientName nvarchar(200));
INSERT INTO @Clients (Rn, Id, ClientName)
SELECT ROW_NUMBER() OVER (ORDER BY Id), Id, ClientName
FROM dbo.Clients WHERE ModifiedBy = @SeedTag;

/* Two catalogues per client. Catalogue is not temporal, so ModifiedBy is our only marker. */
INSERT INTO dbo.Catalogue (Name, ClientId, GeneralNotes, AgicoaClientRef, IMaestroClientCode, CreationDate, ModifiedBy)
SELECT CONCAT(c.ClientName, N' - ', s.Suffix),
       c.Id,
       N'Dummy catalogue',
       CONCAT(N'AGC', RIGHT(N'0000' + CAST(c.Rn AS nvarchar(10)), 4), s.Suffix2),
       CONCAT(N'IMC', RIGHT(N'0000' + CAST(c.Rn AS nvarchar(10)), 4), s.Suffix2),
       SYSUTCDATETIME(),
       @SeedTag
FROM @Clients c
CROSS JOIN (VALUES (N'MAIN CATALOGUE', N'A'), (N'ARCHIVE CATALOGUE', N'B')) AS s(Suffix, Suffix2);

DECLARE @Catalogues TABLE (Rn int, Id int, ClientId int);
INSERT INTO @Catalogues (Rn, Id, ClientId)
SELECT ROW_NUMBER() OVER (PARTITION BY ClientId ORDER BY Id), Id, ClientId
FROM dbo.Catalogue
WHERE ClientId IN (SELECT Id FROM @Clients);

/* Each client is affiliated to a rotating subset of societies. */
DECLARE @Societies TABLE (Rn int, Id int);
INSERT INTO @Societies (Rn, Id)
SELECT ROW_NUMBER() OVER (ORDER BY Id), Id FROM dbo.Society WHERE ModifiedBy = @SeedTag;

DECLARE @SocietyTotal int = (SELECT COUNT(*) FROM @Societies);

INSERT INTO dbo.ClientSociety (ClientsId, SocietiesId)
SELECT DISTINCT c.Id, s.Id
FROM @Clients c
JOIN @Societies s
  ON s.Rn IN (((c.Rn - 1) % @SocietyTotal) + 1,
              ((c.Rn)     % @SocietyTotal) + 1,
              ((c.Rn + 1) % @SocietyTotal) + 1);

------------------------------------------------------------------------------
-- 5. Works
--    Inserted in dependency order so parent ids exist before children.
--    A staging table records what each row is, so titles and links can be
--    generated in one pass afterwards.
------------------------------------------------------------------------------
DECLARE @NewWorks TABLE
(
    Id            int PRIMARY KEY,
    Discriminator varchar(10),
    Title         varchar(1000),
    SeqNo         int,
    OwnerClientRn int
);

DECLARE @TitleWords TABLE (Rn int IDENTITY(1,1), Word nvarchar(50));
INSERT INTO @TitleWords (Word) VALUES
    (N'Midnight'), (N'Harbour'), (N'Crimson'), (N'Silent'), (N'Northern'),
    (N'Hollow'),   (N'Amber'),   (N'Iron'),    (N'Gentle'), (N'Distant'),
    (N'Broken'),   (N'Golden'),  (N'Restless'),(N'Quiet'),  (N'Wandering');

DECLARE @TitleNouns TABLE (Rn int IDENTITY(1,1), Noun nvarchar(50));
INSERT INTO @TitleNouns (Noun) VALUES
    (N'Tide'),    (N'Harvest'), (N'Signal'), (N'Orchard'), (N'Lantern'),
    (N'Passage'), (N'Compass'), (N'Verdict'),(N'Hollow'),  (N'Meridian'),
    (N'Anchor'),  (N'Threshold'),(N'Circuit'),(N'Almanac'), (N'Foundry');

DECLARE @WordCount int = (SELECT COUNT(*) FROM @TitleWords);
DECLARE @NounCount int = (SELECT COUNT(*) FROM @TitleNouns);

/* ---------- 5a. Standalone works ---------- */
DECLARE @i int = 1;
WHILE @i <= @StandaloneCount
BEGIN
    DECLARE @saTitle varchar(1000) =
        (SELECT Word FROM @TitleWords WHERE Rn = ((@i - 1) % @WordCount) + 1) + ' ' +
        (SELECT Noun FROM @TitleNouns WHERE Rn = ((@i * 7 - 1) % @NounCount) + 1);

    INSERT INTO dbo.Works
        (Discriminator, WorksStatus, ProductionYear, FirstBroadcastYear, DurationMinutes,
         WorksTypeId, GenreId, WorksSubTypeId, CompactRef, AS400RefNo, AgicoaWorksReference,
         ColourFormat, Nationality, GeneralNotes, CreationDate, ModifiedBy)
    VALUES
        ('StandAlone',
         CASE WHEN @i % 11 = 0 THEN 2 WHEN @i % 17 = 0 THEN 3 ELSE 1 END,
         1990 + (@i % 35),
         1990 + (@i % 35),
         84 + (@i % 40),
         (SELECT Id FROM @TypeIds    WHERE Rn = ((@i - 1) % @TypeCount) + 1),
         (SELECT Id FROM @GenreIds   WHERE Rn = ((@i - 1) % @GenreCount) + 1),
         (SELECT Id FROM @SubTypeIds WHERE Rn = ((@i - 1) % @SubTypeCount) + 1),
         CONCAT('SA', RIGHT('00000' + CAST(@i AS varchar(10)), 5)),
         CONCAT('AS4SA', RIGHT('00000' + CAST(@i AS varchar(10)), 5)),
         CONCAT('AGW-SA-', RIGHT('00000' + CAST(@i AS varchar(10)), 5)),
         CASE WHEN @i % 5 = 0 THEN 'Black & White' ELSE 'Colour' END,
         CASE WHEN @i % 4 = 0 THEN 'GB' WHEN @i % 4 = 1 THEN 'US' WHEN @i % 4 = 2 THEN 'CA' ELSE 'AU' END,
         'Dummy standalone work',
         SYSUTCDATETIME(), @SeedTag);

    INSERT INTO @NewWorks (Id, Discriminator, Title, SeqNo, OwnerClientRn)
    VALUES (SCOPE_IDENTITY(), 'StandAlone', @saTitle, @i, ((@i - 1) % @ClientCount) + 1);

    SET @i += 1;
END

/* ---------- 5b. Series, then their seasons, then episodes ---------- */
DECLARE @s int = 1;
WHILE @s <= @SeriesCount
BEGIN
    DECLARE @seriesTitle varchar(1000) =
        'The ' +
        (SELECT Word FROM @TitleWords WHERE Rn = ((@s * 3 - 1) % @WordCount) + 1) + ' ' +
        (SELECT Noun FROM @TitleNouns WHERE Rn = ((@s * 5 - 1) % @NounCount) + 1);

    DECLARE @seriesClientRn int = ((@s - 1) % @ClientCount) + 1;

    INSERT INTO dbo.Works
        (Discriminator, WorksStatus, ProductionYear, FirstBroadcastYear,
         WorksTypeId, GenreId, WorksSubTypeId, CompactRef, AS400RefNo, AgicoaWorksReference,
         ColourFormat, Nationality, GeneralNotes, CreationDate, ModifiedBy)
    VALUES
        ('Series', 1, 2005 + (@s % 20), 2005 + (@s % 20),
         @SeriesTypeId,
         (SELECT Id FROM @GenreIds   WHERE Rn = ((@s - 1) % @GenreCount) + 1),
         (SELECT Id FROM @SubTypeIds WHERE Rn = ((@s - 1) % @SubTypeCount) + 1),
         CONCAT('SE', RIGHT('00000' + CAST(@s AS varchar(10)), 5)),
         CONCAT('AS4SE', RIGHT('00000' + CAST(@s AS varchar(10)), 5)),
         CONCAT('AGW-SE-', RIGHT('00000' + CAST(@s AS varchar(10)), 5)),
         'Colour',
         CASE WHEN @s % 3 = 0 THEN 'GB' WHEN @s % 3 = 1 THEN 'US' ELSE 'CA' END,
         'Dummy series', SYSUTCDATETIME(), @SeedTag);

    DECLARE @seriesId int = SCOPE_IDENTITY();

    INSERT INTO @NewWorks (Id, Discriminator, Title, SeqNo, OwnerClientRn)
    VALUES (@seriesId, 'Series', @seriesTitle, @s, @seriesClientRn);

    /* Seasons of this series */
    DECLARE @sn int = 1;
    WHILE @sn <= @SeasonsPerSeries
    BEGIN
        INSERT INTO dbo.Works
            (Discriminator, WorksStatus, ProductionYear, FirstBroadcastYear, Number,
             WorksTypeId, GenreId, WorksSubTypeId, SeriesId,
             CompactRef, AS400RefNo, AgicoaWorksReference, ColourFormat, Nationality,
             GeneralNotes, CreationDate, ModifiedBy)
        VALUES
            ('Season', 1, 2005 + (@s % 20) + @sn, 2005 + (@s % 20) + @sn, @sn,
             @SeriesTypeId,
             (SELECT Id FROM @GenreIds   WHERE Rn = ((@s - 1) % @GenreCount) + 1),
             (SELECT Id FROM @SubTypeIds WHERE Rn = ((@s - 1) % @SubTypeCount) + 1),
             @seriesId,
             CONCAT('SN', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('00' + CAST(@sn AS varchar(10)), 2)),
             CONCAT('AS4SN', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('00' + CAST(@sn AS varchar(10)), 2)),
             CONCAT('AGW-SN-', RIGHT('000' + CAST(@s AS varchar(10)), 3), '-', CAST(@sn AS varchar(10))),
             'Colour',
             CASE WHEN @s % 3 = 0 THEN 'GB' WHEN @s % 3 = 1 THEN 'US' ELSE 'CA' END,
             'Dummy season', SYSUTCDATETIME(), @SeedTag);

        DECLARE @seasonId int = SCOPE_IDENTITY();

        INSERT INTO @NewWorks (Id, Discriminator, Title, SeqNo, OwnerClientRn)
        VALUES (@seasonId, 'Season', CONCAT(@seriesTitle, ' - Season ', @sn), @sn, @seriesClientRn);

        /* Episodes belonging to this season (SeasonId set, Episode_SeriesId left null
           so the episode appears once, under its season). */
        DECLARE @ep int = 1;
        WHILE @ep <= @EpisodesPerSeason
        BEGIN
            INSERT INTO dbo.Works
                (Discriminator, WorksStatus, ProductionYear, FirstBroadcastYear, DurationMinutes, Number,
                 WorksTypeId, GenreId, WorksSubTypeId, SeasonId,
                 CompactRef, AS400RefNo, AgicoaWorksReference, ColourFormat, Nationality,
                 GeneralNotes, CreationDate, ModifiedBy)
            VALUES
                ('Episode',
                 CASE WHEN (@s + @sn + @ep) % 23 = 0 THEN 3 ELSE 1 END,
                 2005 + (@s % 20) + @sn, 2005 + (@s % 20) + @sn, 42 + (@ep % 20), @ep,
                 @SeriesTypeId,
                 (SELECT Id FROM @GenreIds   WHERE Rn = ((@s - 1) % @GenreCount) + 1),
                 (SELECT Id FROM @SubTypeIds WHERE Rn = ((@s - 1) % @SubTypeCount) + 1),
                 @seasonId,
                 CONCAT('EP', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('0' + CAST(@sn AS varchar(10)), 2), RIGHT('00' + CAST(@ep AS varchar(10)), 3)),
                 CONCAT('AS4EP', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('0' + CAST(@sn AS varchar(10)), 2), RIGHT('00' + CAST(@ep AS varchar(10)), 3)),
                 CONCAT('AGW-EP-', CAST(@s AS varchar(10)), '-', CAST(@sn AS varchar(10)), '-', CAST(@ep AS varchar(10))),
                 'Colour',
                 CASE WHEN @s % 3 = 0 THEN 'GB' WHEN @s % 3 = 1 THEN 'US' ELSE 'CA' END,
                 'Dummy episode', SYSUTCDATETIME(), @SeedTag);

            INSERT INTO @NewWorks (Id, Discriminator, Title, SeqNo, OwnerClientRn)
            VALUES (SCOPE_IDENTITY(), 'Episode',
                    CONCAT(@seriesTitle, ' - S', RIGHT('0' + CAST(@sn AS varchar(10)), 2),
                           'E', RIGHT('0' + CAST(@ep AS varchar(10)), 2)),
                    @ep, @seriesClientRn);

            SET @ep += 1;
        END

        SET @sn += 1;
    END

    /* Episodes hung directly off the series with no season. These exercise the
       "Episodes" node in SeasonsEpisodesTree, which reads Episode_SeriesId. */
    DECLARE @le int = 1;
    WHILE @le <= @LooseEpisodes
    BEGIN
        INSERT INTO dbo.Works
            (Discriminator, WorksStatus, ProductionYear, FirstBroadcastYear, DurationMinutes, Number,
             WorksTypeId, GenreId, WorksSubTypeId, Episode_SeriesId,
             CompactRef, AS400RefNo, AgicoaWorksReference, ColourFormat, Nationality,
             GeneralNotes, CreationDate, ModifiedBy)
        VALUES
            ('Episode', 1, 2005 + (@s % 20), 2005 + (@s % 20), 42 + (@le % 20), @le,
             @SeriesTypeId,
             (SELECT Id FROM @GenreIds   WHERE Rn = ((@s - 1) % @GenreCount) + 1),
             (SELECT Id FROM @SubTypeIds WHERE Rn = ((@s - 1) % @SubTypeCount) + 1),
             @seriesId,
             CONCAT('EX', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('000' + CAST(@le AS varchar(10)), 3)),
             CONCAT('AS4EX', RIGHT('000' + CAST(@s AS varchar(10)), 3), RIGHT('000' + CAST(@le AS varchar(10)), 3)),
             CONCAT('AGW-EX-', CAST(@s AS varchar(10)), '-', CAST(@le AS varchar(10))),
             'Colour',
             CASE WHEN @s % 3 = 0 THEN 'GB' WHEN @s % 3 = 1 THEN 'US' ELSE 'CA' END,
             'Dummy series-level episode (no season)', SYSUTCDATETIME(), @SeedTag);

        INSERT INTO @NewWorks (Id, Discriminator, Title, SeqNo, OwnerClientRn)
        VALUES (SCOPE_IDENTITY(), 'Episode',
                CONCAT(@seriesTitle, ' - Special ', @le), @le, @seriesClientRn);

        SET @le += 1;
    END

    SET @s += 1;
END

------------------------------------------------------------------------------
-- 6. Titles
--    Every work gets a Main (TitleType 1) title: several mappings call
--    .First(t => t.TitleType == Main || Episode) and throw without one.
--    Roughly a third also get a MainAlternative (3) so alternate-title
--    searching has something to find.
------------------------------------------------------------------------------
INSERT INTO dbo.WorksTitle (WorksId, Title, ReverseTitle, LanguageCode, TitleType, CreationDate, ModifiedBy)
SELECT w.Id, w.Title, REVERSE(w.Title), N'EN', 1, SYSUTCDATETIME(), @SeedTag
FROM @NewWorks w;

INSERT INTO dbo.WorksTitle (WorksId, Title, ReverseTitle, LanguageCode, TitleType, CreationDate, ModifiedBy)
SELECT w.Id,
       CONCAT(w.Title, ' (Alt)'),
       REVERSE(CONCAT(w.Title, ' (Alt)')),
       CASE WHEN w.Id % 2 = 0 THEN N'FR' ELSE N'DE' END,
       3,
       SYSUTCDATETIME(), @SeedTag
FROM @NewWorks w
WHERE w.Id % 3 = 0;

------------------------------------------------------------------------------
-- 7. Ownership links
--    Every work is owned by a client and filed in that client's first catalogue.
--    Series/season/episode rows inherit the series' owning client so a whole
--    hierarchy stays with one client.
------------------------------------------------------------------------------
INSERT INTO dbo.ClientWorks (ClientsId, WorksId)
SELECT c.Id, w.Id
FROM @NewWorks w
JOIN @Clients c ON c.Rn = w.OwnerClientRn;

INSERT INTO dbo.CatalogueWorks (CataloguesId, WorksId)
SELECT cat.Id, w.Id
FROM @NewWorks w
JOIN @Clients c   ON c.Rn = w.OwnerClientRn
JOIN @Catalogues cat ON cat.ClientId = c.Id AND cat.Rn = 1;

COMMIT TRANSACTION;

------------------------------------------------------------------------------
-- 8. Summary
------------------------------------------------------------------------------
PRINT '--- seeded ---';
SELECT 'Societies'              AS Entity, COUNT(*) AS Rows FROM dbo.Society WHERE ModifiedBy = @SeedTag
UNION ALL SELECT 'Clients',              COUNT(*) FROM dbo.Clients WHERE ModifiedBy = @SeedTag
UNION ALL SELECT 'Catalogues',           COUNT(*) FROM dbo.Catalogue WHERE ModifiedBy = @SeedTag
UNION ALL SELECT 'Works: StandAlone',    COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'StandAlone'
UNION ALL SELECT 'Works: Series',        COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'Series'
UNION ALL SELECT 'Works: Season',        COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'Season'
UNION ALL SELECT 'Works: Episode',       COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'Episode'
UNION ALL SELECT '  of which in season', COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'Episode' AND SeasonId IS NOT NULL
UNION ALL SELECT '  of which series-level', COUNT(*) FROM dbo.Works WHERE ModifiedBy = @SeedTag AND Discriminator = 'Episode' AND Episode_SeriesId IS NOT NULL
UNION ALL SELECT 'Titles',               COUNT(*) FROM dbo.WorksTitle WHERE ModifiedBy = @SeedTag
UNION ALL SELECT 'Client-Society links', COUNT(*) FROM dbo.ClientSociety cs WHERE EXISTS (SELECT 1 FROM dbo.Clients c WHERE c.Id = cs.ClientsId AND c.ModifiedBy = @SeedTag)
UNION ALL SELECT 'Client-Works links',   COUNT(*) FROM dbo.ClientWorks cw WHERE EXISTS (SELECT 1 FROM dbo.Works w WHERE w.Id = cw.WorksId AND w.ModifiedBy = @SeedTag);

/* Sanity check: no work may be both in a season and directly under a series,
   or the SeasonsEpisodesTree would render it twice. */
IF EXISTS (SELECT 1 FROM dbo.Works
           WHERE ModifiedBy = @SeedTag AND SeasonId IS NOT NULL AND Episode_SeriesId IS NOT NULL)
    RAISERROR('Seed produced episodes with both SeasonId and Episode_SeriesId set.', 16, 1);

END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    PRINT 'Seed failed and was rolled back.';
    THROW;
END CATCH
