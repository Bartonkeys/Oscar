CREATE OR ALTER PROCEDURE [dbo].[sp_GetWorksTitleByTitle]
    @Title NVARCHAR(255),
    @SearchType NVARCHAR(50)
AS
BEGIN
    DECLARE @SearchPattern NVARCHAR(255)

    SET @SearchPattern = CASE
        WHEN @SearchType = 'StartsWith' THEN @Title + '%'
        WHEN @SearchType = 'Contains' THEN '%' + @Title + '%'
        WHEN @SearchType = 'Equals' THEN @Title
    END

    IF @SearchType = 'FreeText'
    BEGIN
        SELECT wt.Id, wt.WorksId, wt.Title, wt.TitleType
        FROM WorksTitle wt
        WHERE FREETEXT(wt.Title, @Title);
    END
    ELSE IF @SearchType IN ('StartsWith', 'Equals', 'Contains')
    BEGIN
        SELECT wt.Id, wt.WorksId, wt.Title, wt.TitleType
        FROM WorksTitle wt
        WHERE wt.Title LIKE @SearchPattern;
    END
END