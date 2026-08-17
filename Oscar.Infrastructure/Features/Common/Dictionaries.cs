using System;
namespace Oscar.Infrastructure.Features.Common
{
	public static class Dictionaries
	{
        public static Dictionary<string, string> ClientKnownColumns = new Dictionary<string, string>()
        {
            {"clientName", "ClientName"},
            {"email", "Email"},
            {"clientReference", "ClientReference"},
            {"clientGrade", "ClientGrade"},
            {"status", "Status"},
            {"id", "Id"}
        };

        public static Dictionary<string, string> WorksKnownColumns = new Dictionary<string, string>()
        {
            {"reference", "Reference"},
            {"status", "WorksStatus"},
            {"id", "Id"}
        };

        public static Dictionary<string, string> ReportKnownColumns = new Dictionary<string, string>()
        {
            {"ReportName", "ReportName"},
            {"BaseEntityName", "BaseEntityName"},
            {"Id", "Id"}
        };

        public static Dictionary<string, string> RegistrationKnownColumns = new Dictionary<string, string>()
        {
            {"Notes", "Notes"},
            {"Title", "Title"},
            {"Id", "Id"}

        };

        public static Dictionary<string, string> CountryKnownColumns = new Dictionary<string, string>()
        {
            { "id", "Id" },
            { "name", "Name"},
            { "code", "Code" },
            { "code3A", "Code3A" },
            { "description", "Description"},
            { "inuse", "InUse"}
        };

    }
}

