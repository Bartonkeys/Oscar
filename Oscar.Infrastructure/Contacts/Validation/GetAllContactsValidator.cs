using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Contacts.Queries;

namespace Oscar.Infrastructure.Contacts.Validation
{
    public class GetAllContactsValidator: AbstractValidator<GetAllContactsQuery>
    {
        public GetAllContactsValidator()
        {
            
        }
    }
}
