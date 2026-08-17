using CsvHelper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using System.Globalization;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class AddWorksImportRequestCommandValidator : AbstractValidator<AddWorksImportRequestCommand>
    {
        private OscarContext _context;

        public AddWorksImportRequestCommandValidator(OscarContext context)
        {
            _context = context;

            RuleFor(r => r.WorksImportRequestAddDto).NotNull().WithMessage("Works import request required");
            RuleFor(r => r.WorksImportRequestAddDto.RequestedBy).NotEmpty().WithMessage("Requested by is required");
            RuleFor(r => r.WorksImportRequestAddDto.ClientId).NotEqual(0).WithMessage("Client ID is required");
            RuleFor(r => r.WorksImportRequestAddDto.ClientId).Must(ClientExistsAsync).WithMessage(r => r.GetError()).When(r => r.WorksImportRequestAddDto.FormFile != null);
            RuleFor(r => r.WorksImportRequestAddDto.FormFile).NotNull().WithMessage("File is required");
            //RuleFor(r => r.WorksImportRequestAddDto.FormFile).Must(FileDataIsValid).WithMessage(r => r.GetError()).When(r => r.WorksImportRequestAddDto.FormFile != null);
        }

        private  bool ClientExistsAsync(AddWorksImportRequestCommand addMatchRequestCommand, int clientId)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
            {
                addMatchRequestCommand.SetError($"Client '{clientId}' does not exist");
                return false;
            }
            return true;
        }

        private bool FileDataIsValid(AddWorksImportRequestCommand addMatchRequestCommand, IFormFile? formFile)
        {
            bool result = true;
    
            var extension = Path.GetExtension(formFile?.FileName);
            if(!String.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            {
                addMatchRequestCommand.SetError("File must be in CSV format");
                return false;
            }

            using var reader = new StreamReader(formFile.OpenReadStream());
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<WorksImportDtoMap>();
            var worksImportDtoList = csvReader.GetRecords<WorksImportDto>().ToList();

            int row = 0;
            foreach (var worksImportDto in worksImportDtoList)
            {
                row++;
                worksImportDto.ValidationMessage = string.Empty;

                var validationFailures = new List<string>();

                //WORKS TYPE
                if (string.IsNullOrWhiteSpace(worksImportDto.WorksType)) validationFailures.Add("Works Type required");

                var permittedWorksType = new List<string>() { "stand alone", "series", "season", "episode" };
                if (worksImportDto.WorksType != null && permittedWorksType.Contains(worksImportDto.WorksType.ToLower()))
                {
                    if (string.IsNullOrWhiteSpace(worksImportDto.WorksType)) validationFailures.Add("Works Type invalid");
                }

                if (string.IsNullOrWhiteSpace(worksImportDto.SASeriesNumber))
                {
                    validationFailures.Add("SA/Series # required");
                }
                else
                {
                    if (!int.TryParse(worksImportDto.SASeriesNumber, out _)) validationFailures.Add("SA/Series # must be number");
                }

                if (worksImportDto.WorksType != null && worksImportDto.WorksType.ToLower() == "season")
                {
                    if (string.IsNullOrEmpty(worksImportDto.SeasonNumber))
                    {
                        validationFailures.Add("Season # required");
                    }
                    else if (!int.TryParse(worksImportDto.SeasonNumber, out _))
                    {
                        validationFailures.Add("Season # must be number");
                    }
                }

                else if (worksImportDto.WorksType != null && worksImportDto.WorksType.ToLower() == "episode")
                {
                    if (string.IsNullOrEmpty(worksImportDto.EpisodeNumber))
                    {
                        validationFailures.Add("Episode # required");
                    }
                    else if (!int.TryParse(worksImportDto.EpisodeNumber, out _))
                    {
                        validationFailures.Add("Episode # must be number");
                    }
                }

                if (string.IsNullOrWhiteSpace(worksImportDto.Title)) validationFailures.Add("Title required");

                //PRODUCTION YEAR
                if (string.IsNullOrWhiteSpace(worksImportDto.ProductionYear))
                {
                    validationFailures.Add("Production Year required");
                }
                else
                {
                    int prdYear = 0;
                    if (!int.TryParse(worksImportDto.ProductionYear, out prdYear) || prdYear < 1888 || prdYear > DateTime.Now.Year + 10)
                    {
                        validationFailures.Add($"Production Year must be number from 1888 to {DateTime.Now.Year + 10}");
                    }
                }

                //DURATION
                if (string.IsNullOrWhiteSpace(worksImportDto.Duration))
                {
                    validationFailures.Add("Duration required");
                }
                else
                {
                    int duration = 0;
                    if (!int.TryParse(worksImportDto.Duration, out duration) || duration < 1 || duration > 5000)
                    {
                        validationFailures.Add($"Duration must be number in minutes from 1 to 5000");
                    }
                }

                if (string.IsNullOrWhiteSpace(worksImportDto.DirectorFirstName)) validationFailures.Add("Director First Name required");
                if (string.IsNullOrWhiteSpace(worksImportDto.DirectorLastName)) validationFailures.Add("Director Last Name required");
                if (string.IsNullOrWhiteSpace(worksImportDto.ProductionCompany1)) validationFailures.Add("Production Company 1 required");
                if (string.IsNullOrWhiteSpace(worksImportDto.ProductionCountry1)) validationFailures.Add("Production Country 1 required");

                if (validationFailures.Any())
                {
                    worksImportDto.ValidationMessage = $"Row {row}: {String.Join("; ", validationFailures)}" ;
                }
            }

            if (worksImportDtoList.Any(w => w.ValidationMessage != null && w.ValidationMessage.Length > 0))
            {
                var messageList = worksImportDtoList.Where(w => w.ValidationMessage != null && w.ValidationMessage.Length > 0).Select(w => w.ValidationMessage).ToList();
                var validationMessage = String.Join(Environment.NewLine, messageList);
                addMatchRequestCommand.SetError(validationMessage);
                result = false;
            }

            return result;
        }

    }

    
}
