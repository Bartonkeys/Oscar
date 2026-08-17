CREATE OR ALTER VIEW [dbo].[V_ClientDetails] AS
 WITH ContactDetails AS
 (
	Select ROW_NUMBER() OVER (PARTITION BY ClientId ORDER BY Id) as RowNum, c.ClientId,
		   c.Salutation, c.Title, c.FirstName, c.LastName, 
		   c.Email, c.JobTitle, c.Mobile, c.PeriodStart, c.PeriodEnd, 
		   c.Type, c.CreationDate, c.Website --, c.Commentss		   
	  From dbo.Contact c
	 Where c.ClientId IS NOT NULL
 )
 select distinct
		cl.Id as ClientId, cl.ClientName, 
		case cl.Status
			when 1 then 'Active In Term'
			when 2 then 'Active Lapsed'
			when 3 then 'Passive'
			when 4 then 'NACC'
			when 5 then 'Terminated'
			when 6 then 'Active Consolidated'
			when 7 then 'In Administration'
			when 8 then 'Terminated NFC'
			when 9 then 'Dissolved'
			else 'Undetermined'
		end as ClientStatus,
		cl.ClientReference, cl.ClientType, 
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
		cl.CreationDate as ClientCreatedOn, cl.PeriodStart as ClientStartOn, cl.PeriodEnd as ClientEndOn, 
		cl.Email as ClientEmail, cl.AgicoaClientRef, cl.CRCClientsId, cl.CCCClientsId, cl.MPAAClaimantsId, cl.ScreenRightsPortfolioId,
		cl.IMaestroClientCode as As400RefNum, cl.IMaestroGroupPayeeCode, cl.IMaestroGroupPayeeName,
		-- Contract
		con.AccountingCurrency, IIF(con.AutoRenew = 1, 'Yes', 'No') as AutoRenew, 
		con.ParentCompany, con.Email as ContractEmail, con.FirstStartDate as ContractFirstStartDate, 
		con.CurrentStartDate as ContractCurrentStartDate, 
		IIF(1 = con.Terminated, 'Yes', 'No') as ContractTerminated, 
		con.EndDate as ContractEndDate, --con.Notes as ContractNotes,
		-- Contact
		c.Salutation, c.Title, c.FirstName as ContactFirstName, c.LastName as ContactLastName, c.Email as ContactEmail, c.JobTitle, c.Mobile as ContactMobile, 
		c.PeriodStart as ContactStartDate, c.PeriodEnd as ContactEndDate, c.Type as ContactType, --c.Comments as ContactComments, 
		c.CreationDate as ContactCreationDate, c.Website as ContactWebsite,
		-- Address
		a.AddressLine1, a.AddressLine2, a.AddressLine3, a.AddressLine4, a.Country, a.Email as AddressEmail, a.PostZipCode, a.Website, 
		-- Fields not imported from Felix
		o.FullName as AccountManager
	from dbo.Clients cl
	join dbo.Contract con ON con.Id = cl.ContractId	
	join dbo.Address a ON a.ClientId = cl.Id and a.IsCurrent = 1
	left join ContactDetails c ON c.ClientId = cl.Id and c.RowNum = 1
	left join dbo.CustomerServiceManager csm ON csm.ClientId = cl.Id and csm.IsActive = 1
	left join dbo.Operators o ON o.Id = csm.OperatorId;