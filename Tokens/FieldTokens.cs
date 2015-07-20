using System;
using Orchard.Events;
using Orchard.Fields.Fields;
using Orchard.Localization;
using Orchard.Localization.Services;
using System.Globalization;
using Orchard;
using Orchard.Fields.Tokens;
using MainBit.Fields.Fields;
using MainBit.Fields.Services;

namespace MainBit.Fields.Tokens
{
    public class FieldTokens : ITokenProvider {

        private readonly IDateTimeRangeService _dateTimeRangeService;

        public FieldTokens(
            IDateTimeRangeService dateTimeRangeService) {
            _dateTimeRangeService = dateTimeRangeService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(dynamic context) {

            context.For("DateTimeRangeField", T("Date Time Range Field"), T("Tokens for Date Time Range Fields"))
                .Token("DateTimeRanges", T("Date Time Ranges"), T("The Date Time Ranges."))
                ;

        }

        public void Evaluate(dynamic context) {
            context.For<DateTimeRangeField>("DateTimeRangeField")
                .Token("DateTimeRanges", (Func<DateTimeRangeField, object>)(field => {
                    var viewModel = _dateTimeRangeService.BuildViewModel(field);
                    var valueToDisplay = _dateTimeRangeService.DisplayString(viewModel);
                    return valueToDisplay;
                }))
                ;
        }
    }
}