namespace Oscar.MRIT.Core.Constants
{
    public static class FelixConstants
    {
        public static class Genre
        {
            public const string Fiction = "FI";
            public const string NonFiction = "NF";
            public const string Animation = "AN";
            public const string Unknown = "ZZ";
        }

        public static class GenreSubType
        {
            public const string Factual = "FAC";
            public const string Reality = "REA";
            public const string Infomercial = "INF";
            public const string Sport = "SPO";
            public const string Music = "MUS";
            public const string Comedy = "COM";
            public const string Children = "CHI";
        }

        public static class Retry
        {
            public const int Count = 10;
        }
    }
}
