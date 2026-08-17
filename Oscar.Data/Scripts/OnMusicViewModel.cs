using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Data.Scripts
{
    public static class OnMusicViewModel
    {
        public static string vmOnMusicFelixWorks = @"CREATE VIEW [dbo].[vw_OnMusic_Felix_Works]
AS
select c.id as ClientsId, 
cat.Id as CataloguesId, 
c.ClientName ,
cat.Name as CatalogueName,
w.id as WorksId,
w.AgicoaWorksReference as WorksReference,
ISNULL(w.AS400RefNo, '') AS AS400RefNo, 
CASE WHEN w.Discriminator = 'Episode' THEN w.Number ELSE 0 END AS EpisodeRef,
CASE WHEN W.Discriminator = 'StandAlone' THEN 0 WHEN w.Discriminator = 'Series'  THEN 1 WHEN w.Discriminator = 'Season' THEN 2 WHEN w.Discriminator = 'Episode' THEN 3 END AS SerialLevel,
w.CompactRef,
(select CompactRef from Works where Id = w.SeriesId) as SeriesRef,
(select CompactRef from Works where Id = w.SeasonId) as SeasonRef,
CASE WHEN w.Discriminator = 'Season' THEN w.Number WHEN w.Discriminator = 'Episode' THEN (select Number from Works where Id = w.SeasonId) ELSE 0 END AS SeasonNo,
wt.Name as WorkType,
Case WHEN w.WorksSubTypeId = 2 or w.WorksSubTypeId = 4 THEN 1 ELSE 0 END as Documentary,
w.ProductionYear,
w.FirstBroadcastYear,
w.DurationMinutes as Duration,
w.Isan,
(select Title from WorksTitle wt where WorksId = w.SeriesId and wt.TitleType = 1) as Series_Title,
dbo.fnAllTitles(w.Id) AS Titles,
dbo.fnActors_str(w.Id, '|') AS Actors,
dbo.fnDirectors_str(w.Id, '|')  AS Directors, 
dbo.fnProducers_str(w.Id, '|') AS Producers, 
dbo.fnProductionCompanies_str(w.Id, '|') AS ProductionCompanies,
(select name from Genre where Id = w.GenreId) as Genre, 
(select name from GenreSubType where Id = w.GenreSubTypeId) as GenreSubType, 
w.Nationality,
ISNULL (c.Status, '') AS ClientStatus, 
case when c.ClientGrade = 0 then '' when c.ClientGrade = 1 then 'Bronze' 
when c.ClientGrade = 2 then 'Silver' when c.ClientGrade = 3 then 'Gold'
when c.ClientGrade = 4 then 'Platinum' when c.ClientGrade = 5 then 'Tin' 
when c.ClientGrade = 6 then 'Crossed' when c.ClientGrade = 7 then 'Anthem' else '' end AS ClientGrade, 
ISNULL((SELECT Format(co.EndDate, 'yyyyMMdd')), '') AS ContractEndDate,
co.AutoRenew AS AutoRenewMandate
from ClientWorks cw
join Works w on w.Id = cw.WorksId
join Clients c on c.Id = cw.ClientsId
join Contract co on co.id = c.ContractId
join Catalogue cat on cat.ClientId = c.Id
join WorksType wt on wt.Id = w.WorksTypeId
where co.EndDate >= '2017-12-31 00:00:00.0000000' and (c.Status = 6 or c.Status = 1 or c.Status = 2 or c.Status = 5) ";

        public static string fnActors_str { get; set; } = @"Create function [dbo].[fnActors_str](@id int,@sep as varchar(1))
returns varchar(1000)
begin
declare @list varchar(1000)
set @list = ''
SELECT
	@list = coalesce(@list + a.FirstName + ' ' + a.LastName + @sep, a.FirstName + ' ' + a.LastName) 
FROM 
	Actor a
	inner join ActorWorks aw
	on a.Id = aw.ActorsId 
	and worksid = @id
if @list <> '' set @list = left(@list, len(@list) - 1)
return @list
end";

        public static string fnAllTitles { get; set; } = @"Create FUNCTION [dbo].[fnAllTitles](@WorksId int)
RETURNS nvarchar(4000)

	BEGIN

		DECLARE @list nvarchar(4000)
		SET @list = ''

		SELECT @list = COALESCE(@list + worksTitle.Title + '|', worksTitle.Title )

		FROM 
			worksTitle 

			WHERE WorksId = @WorksId 
			
			ORDER BY TitleType,Title

		--trim off extra vertical bar at end
		IF @list <> '' BEGIN
			SET @list = LTRIM(RTRIM(@list))
			IF RIGHT(@list,1) = '|' SET @list = LEFT(@list, len(@list) - 1)
		END

		RETURN @list

	END";

        public static string fnAllTitlesUpdate { get; set; } = @"ALTER FUNCTION [dbo].[fnAllTitles](@WorksId int)
RETURNS nvarchar(max)

	BEGIN

		RETURN(SELECT Title, LanguageCode
		FROM WorksTitle
		WHERE WorksId = @WorksId 
		FOR JSON AUTO)

	END";

        public static string fnDirectors_str { get; set; } =
            @"Create  function [dbo].[fnDirectors_str](@id int,@sep as varchar(1))
returns varchar(1000)

begin

declare @list varchar(1000)

set @list = ''

SELECT
	@list = coalesce(@list + director.FirstName + ' ' + director.LastName + @sep, director.FirstName + ' ' + director.LastName) 

FROM 
	director 

	inner join DirectorWorks 
	on Director.Id = DirectorWorks.directorsid 
	and worksid = @id

--trim off extra comma at end
if @list <> '' set @list = left(@list, len(@list) - 1)

return @list

end";

        public static string fnProducers_str { get; set; } = @"Create  function [dbo].[fnProducers_str](@id int, @sep as varchar(1))
returns varchar(1000)

begin

declare @list varchar(1000)

set @list = ''

SELECT
	@list = coalesce(@list + producer.FirstName + ' ' + producer.LastName + @sep, producer.FirstName + ' ' + producer.LastName) 

FROM 
	producer 

	inner join ProducerWorks 
	on producer.Id = ProducerWorks.ProducersId 
	and worksid = @id

--trim off extra comma at end
if @list <> '' set @list = left(@list, len(@list) - 1)

return @list

end";

        public static string fnProductionCompanies_str { get; set; } = @"Create  function [dbo].[fnProductionCompanies_str](@id int, @sep as varchar(1))
returns varchar(1000)

begin

declare @list varchar(1000)

set @list = ''

SELECT
	@list = coalesce(@list + company.Name + ' ' + @sep, company.Name) 

FROM 
	Company 

	inner join CompanyWorks 
	on Company.Id = CompanyWorks.CompaniesId 
	and worksid = @id

--trim off extra comma at end
if @list <> '' set @list = left(@list, len(@list) - 1)

return @list

end";
    }
}
