CREATE OR ALTER VIEW [dbo].[V_ClientWorksList] AS
select distinct  
    cl.Id as ClientId, cl.ClientName, ct.Id as CatalogueId, ct.Name as CatalogueName,  
    w.Id as WorksId, w.ProductionYear,  w.Discriminator,   
    (select STRING_AGG(wt1.Title, ', ') from WorksTitle wt1 where wt1.WorksId = w.Id and wt1.TitleType IN (1,2)) AS Titles,  
    (select STRING_AGG(wt1.Title, ', ') from WorksTitle wt1 where wt1.WorksId = w.Id and wt1.TitleType IN (3,4)) AS AlternateTitles,  
    st.Title AS SeasonTitle, srt.Title AS SeriesTitle,  
    (select STRING_AGG(c.Name,', ') from Country c join CountryWorks cw ON cw.WorksId = w.Id where c.Id = cw.CountriesId) as CountriesOfProduction,
    (select STRING_AGG(c.Name,', ') from Company c join CompanyWorks cw ON cw.WorksId = w.Id where c.Id = cw.CompaniesId) as Companies,
    w.AgicoaWorksReference, w.CompactRef, cr.AgicoaDeclarationNumber,w.AS400RefNo,  
    ws.Name as WorksStatus, ws.Id as WorksStatusId,   
    case when ws.Id IN (1,4) then 'Yes' else 'No' end as Released,  
    w.CreationDate, w.LastModified  
from dbo.Clients cl (nolock)  
join dbo.ClientWorks cw (nolock) ON cw.ClientsId = cl.Id  
join dbo.Works w (nolock) ON w.Id = cw.WorksId  
join dbo.CatalogueWorks catw(nolock) on catw.WorksId = w.Id  
join dbo.Catalogue ct (nolock) ON ct.Id = catw.CataloguesId  
join dbo.WorksStatus ws ON ws.Id = w.WorksStatus  
LEFT join dbo.ClientReference cr (nolock) ON cr.WorksId = w.Id
LEFT JOIN dbo.Works s (nolock) ON COALESCE(w.SeasonId, w.Id) = s.Id AND s.Discriminator = 'Season'  
LEFT JOIN dbo.WorksTitle st (nolock) ON st.WorksId = s.Id and st.TitleType IN (1,2)  
LEFT JOIN dbo.Works sr (nolock) ON COALESCE(s.SeriesId, w.SeriesId) = sr.Id AND sr.Discriminator = 'Series'  
LEFT JOIN dbo.WorksTitle srt (nolock) ON sr.Id = srt.WorksId and srt.TitleType IN (1,2);  