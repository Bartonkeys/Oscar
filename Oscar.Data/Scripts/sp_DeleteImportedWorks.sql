CREATE OR ALTER PROCEDURE [dbo].[sp_DeleteImportedWorks]
(
	@WorksImportRequestId INT
) 
AS 
BEGIN
    -- Drop temporary table if exists
    DROP TABLE IF EXISTS #tempWorksToBeDeleted;

    -- Create temporary table
    CREATE TABLE #tempWorksToBeDeleted
    (
        WorksId INT
    );

    -- Clear temporary table
    DELETE FROM #tempWorksToBeDeleted;

    -- Insert relevant Works IDs into temporary table
    INSERT INTO #tempWorksToBeDeleted (WorksId)
    SELECT id 
    FROM Works w 
    WHERE w.WorksImportRequestId = @WorksImportRequestId;

    -- Delete related records from dependent tables
    DELETE FROM ActorWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM ScreenWriterWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM ScriptWriterWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM ProducerWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM DirectorWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM CompanyWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM Conflict 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM CountryWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM Documents 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM LanguageWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM Registration 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM RegistrationBatch 
    WHERE Id IN (
        SELECT b.Id
        FROM RegistrationBatch b 
        LEFT JOIN Registration r ON r.RegistrationBatchId = b.Id
        WHERE r.id IS NULL
    );

    DELETE FROM ChannelRights 
    WHERE RightId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted)))
       OR RightId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT WorksId FROM #tempWorksToBeDeleted));

    DELETE FROM CountryRight 
    WHERE RightsId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted)))
       OR RightsId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT WorksId FROM #tempWorksToBeDeleted));

    DELETE FROM LanguageRights 
    WHERE RightId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted)))
       OR RightId IN (SELECT Id FROM Rights WHERE WorkId IN (SELECT WorksId FROM #tempWorksToBeDeleted));

    DELETE FROM Rights 
    WHERE WorkId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorkId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM CatalogueWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM ClientWorks 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM Mandates 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM SocietyReference 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM DistributorWorks 
    WHERE WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM WorksTitle 
    WHERE WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    DELETE FROM AlternativeTitle
    WHERE WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);
       
    DELETE FROM ClientReference 
    WHERE WorksId IN (SELECT Id FROM Works WHERE Episode_SeriesId IN (SELECT WorksId FROM #tempWorksToBeDeleted))
       OR WorksId IN (SELECT WorksId FROM #tempWorksToBeDeleted);

    -- Delete all dependent episodes
    DELETE FROM Works 
    WHERE Id IN (SELECT WorksId FROM #tempWorksToBeDeleted)
    AND Discriminator = 'Episode';

    -- Delete all dependent seasons
    DELETE FROM Works 
    WHERE Id IN (SELECT WorksId FROM #tempWorksToBeDeleted)
    AND Discriminator = 'Season';

    -- Delete all Series and Standalones
    DELETE FROM Works 
    WHERE Id IN (SELECT WorksId FROM #tempWorksToBeDeleted);
END