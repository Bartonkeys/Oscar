using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Infrastructure.Features.Person.Validation
{
    public class AddPersonCommandValidator<T>: AbstractValidator<AddPersonCommand<T>> where T: PersonEntity
    {
        public AddPersonCommandValidator(OscarContext context)
        {
            RuleFor(r => r.FirstName).NotNull().NotEmpty();
            RuleFor(r => r.LastName).NotNull().NotEmpty();
        }
    }
}
