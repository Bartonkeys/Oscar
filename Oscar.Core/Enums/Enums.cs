using System;
using System.ComponentModel;
namespace Oscar.Core.Enums
{
	public enum Enums
	{
		[Description("client/grades")]
		ClientGrade = 0,

		[Description("client/types")]
		ClientType = 1,

		[Description("client/statuses")]
		Status = 2,

		[Description("country/all")]
		Countries = 3,

		[Description("matching/requestStatus")]
		MatchingRequestStatus = 4,

		[Description("matching/rules")]
		MatchRules = 5,

		[Description("works/genre")]
		WorksGenre = 6,

		[Description("works/language")]
		WorksLanguage = 7,

		[Description("works/status")]
		WorksStatus = 8
	}
}

