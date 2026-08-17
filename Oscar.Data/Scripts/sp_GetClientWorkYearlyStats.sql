CREATE OR ALTER PROCEDURE sp_GetClientWorkYearlyStats
	@CurrentYear INT,
    @ClientId INT = NULL  -- Optional parameter with default value NULL
AS
BEGIN
    DECLARE @Sql NVARCHAR(MAX)

    SET @Sql = N'
    SELECT 
        ClientId,
        ClientName as Client,
        ClientGrade as Grade,
        AccountManager as CSM,
        ClientStatus as Status,
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' THEN Episodes END), 0) AS [' + CAST(@CurrentYear AS NVARCHAR(4)) + '_Episodes],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' THEN Seasons END), 0) AS [' + CAST(@CurrentYear AS NVARCHAR(4)) + '_Seasons],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' THEN Series END), 0) AS [' + CAST(@CurrentYear AS NVARCHAR(4)) + '_Series],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' THEN StandAlones END), 0) AS [' + CAST(@CurrentYear AS NVARCHAR(4)) + '_StandAlones],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 1 THEN Episodes END), 0) AS [' + CAST(@CurrentYear - 1 AS NVARCHAR(4)) + '_Episodes],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 1 THEN Seasons END), 0) AS [' + CAST(@CurrentYear - 1 AS NVARCHAR(4)) + '_Seasons],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 1 THEN Series END), 0) AS [' + CAST(@CurrentYear - 1 AS NVARCHAR(4)) + '_Series],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 1 THEN StandAlones END), 0) AS [' + CAST(@CurrentYear - 1 AS NVARCHAR(4)) + '_StandAlones],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 2 THEN Episodes END), 0) AS [' + CAST(@CurrentYear - 2 AS NVARCHAR(4)) + '_Episodes],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 2 THEN Seasons END), 0) AS [' + CAST(@CurrentYear - 2 AS NVARCHAR(4)) + '_Seasons],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 2 THEN Series END), 0) AS [' + CAST(@CurrentYear - 2 AS NVARCHAR(4)) + '_Series],
        ISNULL(MAX(CASE WHEN CreatedYear = ' + CAST(@CurrentYear AS NVARCHAR(4)) + ' - 2 THEN StandAlones END), 0) AS [' + CAST(@CurrentYear - 2 AS NVARCHAR(4)) + '_StandAlones]
    FROM V_ClientWorkYearlyStats';
	-- Append WHERE clause only if @ClientId is provided
    IF @ClientId IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' WHERE ClientId = ' + CAST(@ClientId AS NVARCHAR(10))
    END

    SET @Sql = @Sql + ' GROUP BY ClientId, ClientName, ClientGrade, AccountManager, ClientStatus;'

    EXEC sp_executesql @Sql
END
