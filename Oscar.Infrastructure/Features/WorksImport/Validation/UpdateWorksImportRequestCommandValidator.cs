using CsvHelper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using System.Globalization;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class UpdateWorksImportRequestCommandValidator : AbstractValidator<ResubmitWorksImportRequestCommand>
    {
        public UpdateWorksImportRequestCommandValidator()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }

    
}
