using CsvHelper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class AddEpisodeImportRequestCommandValidator : AbstractValidator<AddEpisodeImportRequestCommand>
    {
        private OscarContext _context;

        public AddEpisodeImportRequestCommandValidator(OscarContext context)
        {
            _context = context;

            RuleFor(r => r.WorksImportRequestAddDto).NotNull().WithMessage("Works import request required");
            RuleFor(r => r.WorksImportRequestAddDto.RequestedBy).NotEmpty().WithMessage("Requested by is required");
            RuleFor(r => r.WorksImportRequestAddDto.ClientId).NotEqual(0).WithMessage("Client ID is required");
            RuleFor(r => r.WorksImportRequestAddDto.ClientId).Must(ClientExists).WithMessage(r => r.GetError()).When(r => r.WorksImportRequestAddDto.FormFile != null);
            RuleFor(r => r.WorksImportRequestAddDto.FormFile).NotNull().WithMessage("File is required");
            RuleFor(r => r.WorksImportRequestAddDto.FormFile).Must(FileDataIsValid).WithMessage(r => r.GetError()).When(r => r.WorksImportRequestAddDto.FormFile != null);
        }

        private bool ClientExists(AddEpisodeImportRequestCommand addMatchRequestCommand, int clientId)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
            {
                addMatchRequestCommand.SetError($"Client '{clientId}' does not exist");
                return false;
            }
            return true;
        }

        private bool FileDataIsValid(AddEpisodeImportRequestCommand addMatchRequestCommand, IFormFile? formFile)
        {
            bool result = true;
    
            var extension = Path.GetExtension(formFile?.Name);
            if(!String.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            {
                addMatchRequestCommand.SetError("File must be in CSV format");
                return false;
            }

            using var reader = new StreamReader(formFile.OpenReadStream());
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<EpisodeImportDtoMap>();
            var episodeImportDtoList = csvReader.GetRecords<EpisodeImportDto>().ToList();

            int row = 0;
            foreach (var episodeImportDto in episodeImportDtoList)
            {
                row++;
                episodeImportDto.ValidationMessage = string.Empty;

                var validationFailures = new List<string>();

                var seasonId = 0;
                if (!int.TryParse(episodeImportDto.OscarSeasonRef, out seasonId))
                {
                    validationFailures.Add("OSCAR Season Ref must be a number");
                }
                
                var seriesId = 0;
                if (!int.TryParse(episodeImportDto.OscarSeriesRef, out seriesId))
                {
                    validationFailures.Add("OSCAR Series Ref must be a number");
                }

                var episodeNumber = 0;
                if (!int.TryParse(episodeImportDto.EpisodeNumber, out episodeNumber))
                {
                    validationFailures.Add("Episode Number must be a number");
                }
                else if(seriesId > 0 && seasonId > 0 && episodeNumber > 0 &&
                    episodeImportDtoList.Count(e => e.EpisodeNumber == episodeImportDto.EpisodeNumber && e.OscarSeasonRef == episodeImportDto.OscarSeasonRef && e.OscarSeriesRef == episodeImportDto.OscarSeriesRef) > 1)
                {
                    validationFailures.Add("Episode Number can only be used once per season");
                }

                if (seriesId > 0 && seasonId > 0)
                {
                    var season = _context.Seasons.Include(s => s.Episodes).FirstOrDefault(s => s.Id == seasonId && s.SeriesId == seriesId);
                    if(season == null)
                    {
                        validationFailures.Add($"OSCAR Season Ref {seasonId} does not exist in OSCAR Series Ref {seriesId}");
                    }
                    else
                    {
                        if(episodeNumber > 0 && season.Episodes != null && season.Episodes.Any(e => e.Number == episodeNumber))
                        {
                            validationFailures.Add($"OSCAR Season Ref {seasonId} already has an episode {episodeNumber}");
                        }
                    }
                }
                
                if (string.IsNullOrWhiteSpace(episodeImportDto.EpisodeTitle)) validationFailures.Add("Episode Title required");

                if (validationFailures.Any())
                {
                    episodeImportDto.ValidationMessage = $"Row {row}: {String.Join("; ", validationFailures)}" ;
                }
            }

            if (episodeImportDtoList.Any(w => w.ValidationMessage != null && w.ValidationMessage.Length > 0))
            {
                var messageList = episodeImportDtoList.Where(w => w.ValidationMessage != null && w.ValidationMessage.Length > 0).Select(w => w.ValidationMessage).ToList();
                var validationMessage = String.Join(Environment.NewLine, messageList);
                addMatchRequestCommand.SetError(validationMessage);
                result = false;
            }

            return result;
        }

    }
}
