using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common
{
    public abstract class BasePagingQuery
    {
        public int Start { get; set; } = 0;
        [Range(1, 100)]
        public int Take { get; set; } = 20;

        public int Id { get; set; }
       
    }
}
