IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Rights_ClientId_CatalogueId_TypeId' AND object_id = OBJECT_ID('dbo.Rights'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Rights_ClientId_CatalogueId_TypeId] 
    ON [dbo].[Rights] ([ClientId], [CatalogueId], [TypeId]) 
    WITH (ONLINE = ON);
END;
GO
IF NOT EXISTS (SELECT 1    FROM sys.indexes WHERE name = 'IX_Rights_WorksId_TypeId' AND object_id = OBJECT_ID('dbo.Rights'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Rights_WorksId_TypeId 
    ON dbo.Rights (WorkId, TypeId) 
    WITH (ONLINE = ON);
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RegistrationBatch_BatchId' AND object_id = OBJECT_ID('dbo.RegistrationBatch'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_RegistrationBatch_BatchId
    ON dbo.RegistrationBatch (BatchId)
    WITH (ONLINE = ON);
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Registration_ClientId_RegistrationBatchId' AND object_id = OBJECT_ID('dbo.Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Registration_ClientId_RegistrationBatchId 
    ON [dbo].[Registration] ([ClientId], [RegistrationBatchId]) 
    WITH (ONLINE = ON);
END;
GO