using LinqKit;
using Oscar.Core.Enums;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Common
{
    public class EntityChecker
    {
        private readonly OscarContext _context;

        public EntityChecker(OscarContext context)
        {
            _context = context;
        }

        public bool ContactMustExist(int? contactId)
        {
            return _context.Contacts.Any(c => c.Id == contactId);
        }

        public bool WorkStatusMustExist(WorksStatus? workStatus)
        {
            return Enum.IsDefined(typeof(WorksStatus), workStatus);
        }

        public bool GenreMustExist(int? genreId)
        {
            return _context.Genres.Any(c => c.Id == genreId);
        }

        public bool CustomServiceManagerMustExist(int? customServiceManagerId)
        {
            return _context.CustomServiceManagers.Any(c => c.Id == customServiceManagerId);
        }

        public bool SeriesMustExist(int? seriesId)
        {
            return _context.Series.Any(c => c.Id == seriesId);
        }

        public bool SeasonMustExist(int? seasonId)
        {
            return _context.Seasons.Any(c => c.Id == seasonId);
        }

        public bool TitleMustNotExist(ICollection<WorksTitleDto> titleDtoList)
        {
            return !_context.WorksTitles.Any(c => titleDtoList.Select(t => t.Title).Contains(c.Title));
        }

    }
}
