using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.ScriptWriter.Queries;

namespace Oscar.Infrastructure.Features.ScriptWriter.Validation
{
    public class GetAllScriptWritersQueryValidation : AbstractValidator<GetAllScriptWritersQuery>
    {
        public GetAllScriptWritersQueryValidation()
        {

        }
    }
}
