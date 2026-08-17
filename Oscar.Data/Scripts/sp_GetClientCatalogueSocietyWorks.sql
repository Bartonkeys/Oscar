CREATE OR ALTER PROCEDURE [dbo].[sp_GetClientCatalogueSocietyWorks]
(
	@ClientId INT = NULL,
	@CatalogueId INT = NULL,
	@SocietyId INT,
	@IncludePreviouslyRegistered BIT
)AS 
BEGIN
    DECLARE @WorksId INT;

    DROP TABLE IF EXISTS #tempworks
    CREATE TABLE #tempworks (worksid INT,
    parentworksid INT,
    discriminator VARCHAR(50),
	catalogueId INT)

	--Insert Series and StandAlones
    INSERT INTO #tempworks(worksid, parentworksid, discriminator, catalogueId)
    SELECT DISTINCT w.id, w.id, w.discriminator, ctw.CataloguesId
    FROM works w 
    JOIN clientworks cw ON w.id = cw.worksid
	JOIN CatalogueWorks ctw on ctw.WorksId = w.Id
    WHERE(cw.ClientsId = @ClientId OR @ClientId IS NULL)
	AND (ctw.CataloguesId = @CatalogueId OR @CatalogueId IS NULL)
	AND w.discriminator IN ('Series', 'StandAlone') 
	AND w.WorksStatus in (1, 5)

	-- Insert seasons for each series
    INSERT INTO #tempworks(worksid, parentworksid, discriminator, catalogueId)
    SELECT DISTINCT w.id, w.SeriesId, w.discriminator, ctw.CataloguesId
    FROM works w 
    JOIN clientworks cw ON w.id=cw.worksid
	JOIN CatalogueWorks ctw on ctw.WorksId = w.Id
    JOIN #tempworks t ON t.worksid=w.SeriesId 
    WHERE (cw.ClientsId = @ClientId OR @ClientId IS NULL)
	AND (ctw.CataloguesId = @CatalogueId OR @CatalogueId IS NULL)
	AND t.discriminator = 'Series'
	AND w.discriminator IN ('Season')
	AND w.WorksStatus in (1, 5)

	-- Insert episodes for each season
    INSERT INTO #tempworks(worksid, parentworksid, discriminator, catalogueId)
    SELECT DISTINCT w.id, w.SeasonId, w.discriminator, ctw.CataloguesId
    FROM works w 
    JOIN clientworks cw ON w.id=cw.worksid
	JOIN CatalogueWorks ctw on ctw.WorksId = w.Id
    JOIN #tempworks t ON t.worksid=w.SeasonId
    WHERE (cw.ClientsId = @ClientId OR @ClientId IS NULL)
	AND (ctw.CataloguesId = @CatalogueId OR @CatalogueId IS NULL)
	AND t.discriminator = 'Season'
	AND w.discriminator IN ('Episode')
	AND w.WorksStatus in (1, 5)

    IF(@IncludePreviouslyRegistered IS NULL OR @IncludePreviouslyRegistered='False')
	BEGIN 
		--Exclude works which are already registered
        DELETE t
        FROM #tempworks t
        JOIN registration r ON r.worksid=t.worksid
        WHERE (r.clientid = @ClientId OR @ClientId IS NULL)
		AND (r.catalogueid = @CatalogueId OR @CatalogueId IS NULL)
		AND r.societyid = @SocietyId 
		AND r.registerstatus = 4 
    END
   
	SELECT worksid, parentworksid, discriminator, catalogueId
	FROM #tempworks t
	order by parentworksid, worksid
END