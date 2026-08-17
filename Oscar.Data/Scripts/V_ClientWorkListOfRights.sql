CREATE OR ALTER VIEW [dbo].[V_ClientWorkListOfRights] AS
	select distinct
		   cl.Id as ClientId, cl.ClientName, ct.Id as CatalogueId, ct.Name as CatalogueName,
		   w.Id as WorksId, w.Discriminator, 
		   (select STRING_AGG(wt1.Title, ', ') from WorksTitle wt1 where wt1.WorksId = w.Id and wt1.TitleType IN (1,2)) AS Titles,
		   (select STRING_AGG(wt1.Title, ', ') from WorksTitle wt1 where wt1.WorksId = w.Id and wt1.TitleType IN (3,4)) AS AlternateTitles,
		   w.ProductionYear, w.CompactRef, 
		   rt.Name as RightsType, r.StartOfRight as StartDate, r.EndOfRight as EndDate, r.Percentage, 
		   c.Name as Country, 
		   l.Name as Language, ch.Name as Channel,	   
		   case when ws.Id IN (1,4) then 'Yes' else 'No' end as Released	   
	from dbo.Clients cl
	join dbo.ClientWorks cw ON cw.ClientsId = cl.Id
	join dbo.Works w ON w.Id = cw.WorksId
	join dbo.Catalogue ct ON ct.ClientId = cl.Id 
	join dbo.WorksStatus ws ON ws.Id = w.WorksStatus
	join dbo.Rights r ON r.ClientId = cw.ClientsId and r.WorkId = cw.WorksId and r.CatalogueId = ct.Id
	join dbo.RightsType rt ON rt.Id = r.TypeId 
	join dbo.CountryRight cr ON cr.RightsId = r.Id
	join dbo.Country c ON c.Id = cr.CountriesId
	join dbo.LanguageRights lr ON lr.RightId = r.Id
	join dbo.Language l ON l.Id = lr.LanguageId
	join dbo.ChannelRights chr ON chr.RightId = r.Id
	join dbo.Channel ch ON ch.Id = chr.ChannelId;
