using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.CustomServiceManager.Commands;
using Oscar.Infrastructure.Features.CustomServiceManager.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class CustomerServiceManagers
    {
        private List<OperatorDto>? _operators;
        private OperatorDto? _operator;
        private CustomerServiceManagerDto? _customServiceManager;
        private String _fullName;

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public ICollection<CustomerServiceManagerDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<CustomerServiceManagerDto>> ValueChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAllOperators();
            StateHasChanged();
        }

        private async Task LoadAllOperators()
        {
            _operators = (await Mediator.Send(new GetAllOperatorsQuery())).Value.OrderBy(x => x.FullName).ToList();
            _operators.RemoveAll(x => Value.Select(c => c.Id).ToList().Contains(x.Id));
        }

        private async Task<IEnumerable<OperatorDto>> Search(string value, CancellationToken token)
        {
            if(_operators == null) await LoadAllOperators();

            if (string.IsNullOrWhiteSpace(value))
                return _operators;

            return _operators.Where(x => x.FullName.Contains(value.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        private async void AddCustomServiceManager()
        {
            if (_operator != null && !Value.Any(c => c.Operator.Id == _operator.Id && c.IsActive))
            {
                var customServiceManager = new CustomerServiceManagerDto
                {
                    Operator = _operator,
                    IsActive = true
                };
                Value.Add(customServiceManager);
                _operators.Remove(_operator);
            }
            _customServiceManager = default;
        }

        private async void RemoveCustomServiceManager(CustomerServiceManagerDto customerServiceManagerDto)
        {
            if (customerServiceManagerDto.IsActive)
                customerServiceManagerDto.IsActive = false;
            else
                Value.Remove(customerServiceManagerDto);

            if(_operators.All(o => o.Id != customerServiceManagerDto.Operator.Id))
                _operators.Add(customerServiceManagerDto.Operator);
        }

        private async void CreateOperator()
        {
            var addOperatorCommand = new AddOperatorCommand()
            {
                OperatorDto = _operator
            };
            var result = await Mediator.Send(addOperatorCommand);
            await HandleResult<OperatorDto>(result);
        }

        private async Task HandleResult<T>(Result<OperatorDto> result) where T : OperatorDto
        {
            if (result.IsSuccess)
            {
                _operators.Add(result.Value);
                StateHasChanged();
                Snackbar.Add("Successfully created", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private String? ListItemString(OperatorDto? @operator)
        {
            String? listItem = null;
            if(@operator != null)
            {
                listItem = @operator.FullName;
            }
            return listItem;
        }

        private String? ListItemString(CustomerServiceManagerDto? customerServiceManager)
        {
            String? listItem = null;
            if (customerServiceManager != null)
            {
                listItem = customerServiceManager?.Operator?.FullName;
            }
            return listItem;
        }

        //This is to disable Add/Create functionality so that we can restrict attaching only 1 CSM to respective client
        private bool AllowAddOrCreateCSM()
        {
            return Value.Count == 0 || Value.All(c => c.IsActive == false);
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

