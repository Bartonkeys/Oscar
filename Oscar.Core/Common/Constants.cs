namespace Oscar.Core.Common
{
    public static class Constants
    {
        public static class Rights
        {
            public static DateTime Perpetuity = new(9999, 12, 31);
        }

        //Use some fixed guid for batch id for all manually added registrations
        public const string ManualEntryRegistrationBatchId = "7935a3cd-e649-47e9-a9ef-078bc767af52";
    }
}
