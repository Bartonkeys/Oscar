ALTER VIEW [dbo].[vw_OnMusic_Felix_Works]
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
join CatalogueWorks catw on catw.WorksId = w.Id
join Catalogue cat on cat.Id = catw.CataloguesId
join WorksType wt on wt.Id = w.WorksTypeId
where co.EndDate >= '2017-12-31 00:00:00.0000000' and (c.Status = 6 or c.Status = 1 or c.Status = 2 or c.Status = 5)