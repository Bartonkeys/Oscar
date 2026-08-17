namespace Oscar.Core.Enums
{
    public enum FeatureEvent
    {
        ValidationFail = 1
    }
    public enum StandAloneFeatureEvent
    {
        Get = 100,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteNotFound,
        Delete
    }

    public enum SeriesFeatureEvent
    {
        Get = 120,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteNotFound,
        Delete,
        CopyNotFound
    }

    public enum RightFeatureEvent
    {
        Add,
        Update,
        Delete
    }

    public enum SeasonFeatureEvent
    {
        Get = 140,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteNotFound,
        CopyNotFound,
        Delete,
        Copy
    }

    public enum EpisodeFeatureEvent
    {
        Get = 160,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteNotFound,
        Delete,
        Copy,
        BulkAdd
    }

    public enum WorksFeatureEvent
    {
        Get = 180,
        GetNotFound,
        GetBadRequest,
        GetGenre,
        GetGenreSubType,
        GetWorksSubType,
        GetType,
        GetLanguage,
        GetWorksStatus,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteBadRequest,
        Delete,
        GetFromCache
    }

    public enum ClientFeatureEvent
    {
        Get = 200,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteBadRequest,
        Delete,
        GetFromCache
    }

    public enum MatchRequestFeatureEvent
    {
        Get = 220,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        UpdateNotFound,
        DocumentNotFound,
        DocumentNotExported
    }

    public enum MatchResultFeatureEvent
    {
        Get = 240,
        GetNotFound,
        GetBadRequest
    }

    public enum StaticDataFeatureEvent
    {
        Get = 260,
        GetNotFound,
        GetBadRequest
    }

    public enum ImportExportServiceEvent
    {
        ImportSuccess = 280,
        ExportSuccess,
        ImportError,
        ExportError
    }

    public enum WorksImportRequestFeatureEvent
    {
        Get = 300,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        UpdateNotFound,
        DocumentNotFound,
        DocumentNotExported,
        ValidationFailed,
        DeleteNotFound,
        ClientNotFound,
    }

    public enum AzureStorage
    {
        QueueSend = 320,
        BlobUpload,
        BlobDelete
    }

    public enum FunctionEvent
    {
        Match = 340,
        MatchError,
        WorksImport,
        WorksImportError,
        Registration,
        RegistrationError,
        EquivalenceProcessor,
        EquivalenceError,
        ScreenrightsProcessor,
        ScreenrightsError


    }

    public enum ReportFeatureEvent
    {
        Get = 360,
        GetNotFound,
        GetBadRequest,
        GetBaseEntities,
        Add,
        AddConflict,
        AddBadRequest,
        Update,
        UpdateConflict,
        UpdateNotFound,
        UpdateBadRequest,
        DeleteBadRequest,
        DeleteNotFound,
        Delete
    }

    public enum CatalogueFeatureEvent
    {
        Get = 380,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete,
        DeleteBadRequest,
        DeleteNotFound,
        UpdateNotFound,
        UpdateBadRequest
    }

    public enum ProducerFeatureEvent
    {
        Get = 400,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum DirectorFeatureEvent
    {
        Get = 420,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum ActorFeatureEvent
    {
        Get = 440,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum DistributorFeatureEvent
    {
        Get = 460,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum ScreenWriterFeatureEvent
    {
        Get = 480,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum CompanyFeatureEvent
    {
        Get = 500,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete,
        GetFromCache
    }

    public enum CountryFeatureEvent
    {
        Get = 520,
        GetFromCache,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum MandateTypeFeatureEvent
    {
        Get = 520,
        GetFromCache,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum CountryGrouopFeatureEvent
    {
        Get = 520,
        GetFromCache,
        GetNotFound,
        GetBadRequest
    }

    public enum RegistrationFeatureEvent
    {
        Get = 500,
        GetNotFound,
        GetBadRequest,
        Add,
        AddedToQueue,
        AddConflict,
        AddBadRequest,
        Error,
        BatchNotFound,
        ErrorsWithinBatch,
        BatchComplete,
        BatchExportFailed,
        DeleteBadRequest,
        DeleteNotFound,
        Delete,
        Update
    }

    public enum SocietyReferenceFeatureEvent
    {
        Add,
        Delete,
        Update,
        DeleteNotFound,
        Get
    }

    public enum ConflictFeatureEvent
    {
        Add,
        Delete,
        Update,
        DeleteNotFound
    }

    public enum WorksTypeEvent
    {
        Get = 520,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum ScriptWriterFeatureEvent
    {
        Get = 540,
        GetNotFound,
        GetBadRequest,
        GetFromCache,
        Add,
        Update,
        Delete
    }

    public enum SocietyFeatureEvent
    {
        Get = 540,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum ChannelFeatureEvent
    {
        Get = 520,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum EquivalenceRequestFeatureEvent
    {
        Get = 540,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        UpdateNotFound,
        DocumentNotFound,
        DocumentNotExported
    }

    public enum DocumentFeatureEvent
    {
        Get = 560,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        UpdateNotFound,
        DocumentNotFound,
        DocumentNotExported,
        DeleteDocument,
        DeleteAzureBlob
    }

    public enum ScreenrightsRequestFeatureEvent
    {
        Get = 580,
        GetNotFound,
        GetBadRequest,
        Add,
        AddConflict,
        AddBadRequest,
        UpdateNotFound,
        DocumentNotFound,
        DocumentNotExported
    }

    public enum ContactFeatureEvent
    {
        Get = 600,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum CustomServiceManagerFeatureEvent
    {
        Get = 620,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

    public enum OperatorFeatureEvent
    {
        Get = 620,
        GetNotFound,
        GetBadRequest,
        Add,
        Update,
        Delete
    }

}