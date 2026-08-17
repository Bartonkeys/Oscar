using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;
using FluentValidation;
using static Oscar.Blazor.Library.Components.Imports.CreateImportForm;
using static MudBlazor.CategoryTypes;
using MudBlazor;
using Oscar.Core.Entities;
using System.Collections.Generic;
using LinqKit;
using Oscar.Infrastructure.Features.Clients;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Clients.Validation;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class ClientRightsForm
    {
        private bool _loading = true;
        private MudTable<RightDto> table;
        private int totalItems;
        private string _searchString = null;
        private IEnumerable<RightDto> _rights;
        private RightDto _selectedItem;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        protected override async Task OnInitializedAsync() => await Task.Run(LoadRights);
        protected override async Task OnParametersSetAsync() => await Task.Run(LoadRights);

        private async Task LoadRights()
        {
            var result = (await Mediator.Send(new GetRightsByClientIdQuery()
            {
                ClientId = Id,
            }));

            if (result.IsFailure) return;

            _rights = result.Value;
            _loading = false;
        }

        private bool Filter(RightDto right) => FilterBySearchString(right, _searchString);

        private static bool FilterBySearchString(RightDto right, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(right.Type.Name)
                   || right.Type.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || (!string.IsNullOrWhiteSpace(right.Type.Description) && right.Type.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                   || right.StartOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.EndOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.StartOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.EndOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }
    }
}