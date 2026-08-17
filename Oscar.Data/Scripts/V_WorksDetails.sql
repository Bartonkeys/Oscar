CREATE OR ALTER VIEW [dbo].[V_WorksDetails]
AS
SELECT Id AS WorksId,
                 (SELECT STRING_AGG(c.ClientName, ', ') AS Expr1
                 FROM    dbo.Clients AS c INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = c.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS Client,
                 (SELECT STRING_AGG(cat.Name, ', ') AS Expr1
                 FROM    dbo.Catalogue AS cat INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS Catalogue,
                 (SELECT STRING_AGG(CASE c.Status WHEN 1 THEN 'Active_In_Term' WHEN 2 THEN 'Active_Lapsed' WHEN 3 THEN 'Passive' WHEN 4 THEN 'NACC' WHEN 5 THEN 'Terminated' WHEN 6 THEN 'Active_Consolidated' WHEN 7 THEN 'In_Administration' WHEN 8 THEN 'Terminated_NFC' WHEN
                               9 THEN 'Dissolved' ELSE 'Unknown' END, ', ') AS Expr1
                 FROM    dbo.Clients AS c INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = c.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS ClientStatus,
                 (SELECT STRING_AGG(CASE c.ClientGrade WHEN 0 THEN 'None' WHEN 1 THEN 'Bronze' WHEN 2 THEN 'Silver' WHEN 3 THEN 'Gold' WHEN 4 THEN 'Platinum' WHEN 5 THEN 'Tin' WHEN 6 THEN 'Crossed' WHEN 7 THEN 'Anthem' ELSE 'Unknown' END, ', ') AS Expr1
                 FROM    dbo.Clients AS c INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = c.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS ClientGrade,
                 (SELECT STRING_AGG(o.FullName, ',') AS Expr1
                 FROM    dbo.Clients AS cl INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = cl.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id LEFT OUTER JOIN
                              dbo.CustomerServiceManager AS csm ON csm.ClientId = cl.Id AND csm.IsActive = 1 LEFT OUTER JOIN
                              dbo.Operators AS o ON o.Id = csm.OperatorId
                 WHERE (cw.WorksId = w.Id)) AS CSM,
                 (SELECT STRING_AGG(cl.IMaestroClientCode, ',') AS Expr1
                 FROM    dbo.Clients AS cl INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = cl.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS ClientAs400RefNum,
                 (SELECT STRING_AGG(cl.IMaestroGroupPayeeCode, ',') AS Expr1
                 FROM    dbo.Clients AS cl INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = cl.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS IMaestroGroupPayeeCode,
                 (SELECT STRING_AGG(cl.IMaestroGroupPayeeName, ',') AS Expr1
                 FROM    dbo.Clients AS cl INNER JOIN
                              dbo.Catalogue AS cat ON cat.ClientId = cl.Id INNER JOIN
                              dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS IMaestroGroupPayeeName, Discriminator AS WorksType,
                 (SELECT STRING_AGG(Title, ', ') AS Expr1
                 FROM    dbo.WorksTitle AS t
                 WHERE (WorksId = w.Id) AND (TitleType = 1 OR
                              TitleType = 2)) AS Title,
                 (SELECT STRING_AGG(Title, ', ') AS Expr1
                 FROM    dbo.WorksTitle AS t
                 WHERE (WorksId = w.Id) AND (TitleType = 3 OR
                              TitleType = 4)) AS AltTitle,
                 (SELECT Title
                 FROM    dbo.WorksTitle
                 WHERE (WorksId = w.SeasonId) AND (TitleType = 1 OR
                              TitleType = 2)) AS SeasonName,
                 (SELECT Title
                 FROM    dbo.WorksTitle AS WorksTitle_1
                 WHERE (WorksId = w.SeriesId) AND (TitleType = 1 OR
                              TitleType = 2)) AS SeriesName,
                 (SELECT STRING_AGG(c.Name, ',') AS Expr1
                 FROM    dbo.CountryWorks AS cw INNER JOIN
                              dbo.Country AS c ON c.Id = cw.CountriesId
                 WHERE (cw.WorksId = w.Id)) AS ProductionCountries,
                 (SELECT STRING_AGG(c.Name, ',') AS Expr1
                 FROM    dbo.CompanyWorks AS cw INNER JOIN
                              dbo.Company AS c ON c.Id = cw.CompaniesId
                 WHERE (cw.WorksId = w.Id)) AS Companies, ProductionYear, DurationMinutes AS Duration, AgicoaWorksReference AS AgicoaReference,
                 (SELECT STRING_AGG(AgicoaDeclarationNumber, ',') AS Expr1
                 FROM    dbo.ClientReference
                 WHERE (WorksId = w.Id)) AS DeclarationNo, CompactRef, AS400RefNo, (CASE w.WorksStatus WHEN - 1 THEN 'Any' WHEN 1 THEN 'Active' WHEN 2 THEN 'Uncontrolled' WHEN 3 THEN 'Incomplete' WHEN 4 THEN 'Relinquished' WHEN 5 THEN 'InConflict' ELSE 'Unknown' END) 
             AS WorksStatus, CreationDate,
          (SELECT STRING_AGG(d.FirstName + ' ' + d.LastName, ', ') AS Expr1
                 FROM    dbo.Director AS d INNER JOIN
                              dbo.DirectorWorks AS dw ON dw.DirectorsId = d.Id
                 WHERE (dw.WorksId = w.Id)) AS Directors,
				(SELECT STRING_AGG(FORMAT(con.EndDate, 'dd/MM/yyyy'), ',') AS Expr1
                 FROM dbo.Contract as con
				 INNER JOIN dbo.Clients AS cl on cl.ContractId = con.Id
				 INNER JOIN dbo.Catalogue AS cat ON cat.ClientId = cl.Id
				 INNER JOIN dbo.CatalogueWorks AS cw ON cw.CataloguesId = cat.Id
                 WHERE (cw.WorksId = w.Id)) AS ContractEndDate,
				 (SELECT STRING_AGG(FORMAT(r.EndOfRight, 'dd/MM/yyyy'), ',') AS Expr1
					from Rights as r
					where (r.WorkId = w.Id and TypeID = 1 and r.Percentage > 0)
				 ) as RightsEndDate,
				(SELECT STRING_AGG(c.Name, ',') AS Expr1
					from Country as c
					join CountryRight as cr on cr.CountriesId = c.Id
					join Rights as r on r.Id = cr.RightsId
					where (r.WorkId = w.Id and TypeID = 1 and r.Percentage = 0)
				 ) as TerritoriesExcluded,
			(SELECT STRING_AGG(r.Percentage, ',') AS Expr1
					from Rights as r
					where (r.WorkId = w.Id and TypeID = 1 and r.Percentage > 0)
				 ) as PercentageClaimed
FROM   dbo.Works AS w
GROUP BY Id, Discriminator, WorksTypeId, SeasonId, SeriesId, ProductionYear, DurationMinutes, AgicoaWorksReference, CompactRef, AS400RefNo, WorksStatus, CreationDate