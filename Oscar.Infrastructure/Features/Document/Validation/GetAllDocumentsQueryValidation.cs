using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Document.Queries;

namespace Oscar.Infrastructure.Features.Document.Validation
{
    public class GetAllDocumentsQueryValidation: AbstractValidator<GetAllDocumentsQuery>
    {
        public GetAllDocumentsQueryValidation()
        {

        }
    }
}
