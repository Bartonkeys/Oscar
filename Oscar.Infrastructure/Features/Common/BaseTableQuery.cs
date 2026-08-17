using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common
{
    public abstract class BaseTableQuery
    {
        public int Start { get; set; } = 0;
        [Range(1, 100)]
        public int Take { get; set; } = 20;

        public string? SortColumn { get; set; } = "Id";
        public string? SortDirection { get; set; } = "ascending";
        public string? BaseEntityName { get; set; } = "NOT_SET";

        public List<SearchObject> SearchObjects { get; set; } = new List<SearchObject>();
        public List<SelectObject> SelectObjects { get; set; } = new List<SelectObject>();


    }
}
