CREATE OR ALTER VIEW [dbo].[V_ClientProductionCountries] AS
 select distinct 
		w.Id as WorksId,
		cl.Id as ClientId,	cl.ClientName, ct.Id as CatalogueId, ct.Name as CatalogueName,
		(select STRING_AGG(wt1.Title, ', ') from WorksTitle wt1 where wt1.TitleType IN(1,2) and wt1.WorksId = w.Id) as Title, 
		(select STRING_AGG(wt2.Title, ', ') from WorksTitle wt2 where wt2.TitleType IN(3,4) and wt2.WorksId = w.Id) as AlternateTitle,
		w.CompactRef, w.Discriminator, 
		(select STRING_AGG(c.Name,', ') from Country c join CountryWorks cw ON cw.WorksId = w.Id where c.Id = cw.CountriesId) as CountriesOfProduction,
		ws.Description as WorksStatus, ws.Id as WorksStatusId, --wt.Name as WorkType,
		case when ws.Id IN (1,4) then 'Yes' else 'No' end as Released
   from dbo.ClientWorks cw
   join dbo.Works w ON w.Id = cw.WorksId  
   join dbo.Clients cl ON cl.Id = cw.ClientsId    
   join CatalogueWorks ctw ON ctw.WorksId = cw.WorksId
   join Catalogue ct ON ct.Id = ctw.CataloguesId and ct.ClientId = cl.Id
   --join WorksType wt on wt.Id = w.WorksTypeId
   join dbo.WorksStatus ws ON ws.Id = w.WorksStatus;
  