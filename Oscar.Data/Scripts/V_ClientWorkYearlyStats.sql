CREATE OR ALTER VIEW [dbo].[V_ClientWorkYearlyStats] AS
WITH YEARLY_STATS AS (
	SELECT
		cw.ClientsId AS ClientId,
		DATEPART(YEAR, w.CreationDate) AS CreatedYear,
		COUNT(CASE WHEN w.Discriminator = 'StandAlone' THEN 1 END) AS StandAlones,
		COUNT(CASE WHEN w.Discriminator = 'Series' THEN 1 END) AS Series,
		COUNT(CASE WHEN w.Discriminator = 'Episode' THEN 1 END) AS Episodes,
		COUNT(CASE WHEN w.Discriminator = 'Season' THEN 1 END) AS Seasons
	FROM ClientWorks cw
	JOIN Works w ON cw.WorksId = w.Id
	GROUP BY cw.ClientsId, DATEPART(YEAR, w.CreationDate)
	)
 select distinct
		cl.Id as ClientId, cl.ClientName, 
		case cl.ClientGrade
			when 0 then 'None'
			when 1 then 'Bronze'
			when 2 then 'Silver'
			when 3 then 'Gold'
			when 4 then 'Platinum'
			when 5 then 'Tin'
			when 6 then 'Crossed'
			when 7 then 'Anthem'
			else 'Undefined'
		end as ClientGrade,
		o.FullName as AccountManager,
		case cl.Status
			when 1 then 'Active (In Term)'
			when 2 then 'Active (Lapsed)'
			when 3 then 'Passive'
			when 4 then 'NACC'
			when 5 then 'Terminated'
			when 6 then 'Active (Consolidated)'
			when 7 then 'In Administration'
			when 8 then 'Terminated (NFC)'
			when 9 then 'Dissolved'
			else 'Undetermined'
		end as ClientStatus,
		ys.CreatedYear,
		ys.Episodes,
		ys.Seasons,
		ys.Series,
		ys.StandAlones		
   FROM dbo.Clients cl
   JOIN YEARLY_STATS ys ON ys.ClientId = cl.Id
   LEFT JOIN dbo.CustomerServiceManager csm ON csm.ClientId = cl.Id AND csm.IsActive = 1
   LEFT JOIN dbo.Operators o ON o.Id = csm.OperatorId;
