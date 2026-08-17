using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class RightsUpdateGrid
    {
        private RightDto? selectedItem1;
        private bool _rightsPerpetuity;
        private bool _rightsValidityPerpetuity;
        private DateTime? _endOfRight;
        private DateTime? _endOfValidity;
        private DateTime? _startOfRight;
        private DateTime? _startOfValidity;

        [Parameter]
        public List<RightDto>? Rights { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        private void BackupItem(object element)
        {
            if (!(element is RightDto right)) return;

            _startOfRight = right.StartOfRight;
            _startOfValidity = right.StartOfValidity;

            if(!right.RightsPerpetuity)
                _endOfRight = right.EndOfRight;

            if(!right.ValidityPerpetuity)
                _endOfValidity = right.EndOfValidity;
            _rightsPerpetuity = right.RightsPerpetuity;
            _rightsValidityPerpetuity = right.ValidityPerpetuity;
        }

        private void ResetItemToOriginalValues(object obj)
        {
            
        }

        private void ItemHasBeenCommitted(object obj)
        {
            if (!(obj is RightDto amendedRight)) return;

            var right = Rights.Single(r => r.Id == amendedRight.Id);
            if (right == null) return;

            right.Percentage = amendedRight.Percentage;
            right.StartOfRight = _startOfRight ?? right.StartOfRight;
            right.StartOfValidity = _startOfValidity ?? right.StartOfValidity;
            right.EndOfRight = !_rightsPerpetuity && _endOfRight != null ? _endOfRight.Value : new DateTime(9999, 12, 31);
            right.EndOfValidity = !_rightsValidityPerpetuity && _endOfValidity != null ? _endOfValidity.Value : new DateTime(9999, 12, 31);
            right.Countries = amendedRight.Countries;
            right.ChannelRights = amendedRight.ChannelRights;
            right.LanguageRights = amendedRight.LanguageRights;

            StateHasChanged();
        }
    }
}