using MainBit.Fields.Fields;
using MainBit.Fields.Settings;
using MainBit.Fields.ViewModels;
using Orchard;
using Orchard.Core.Common.ViewModels;
using Orchard.Localization;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MainBit.Fields.Services
{
    public interface IDateTimeRangeService : IDependency
    {
        DateTimeRangeFieldViewModel BuildViewModel(DateTimeRangeField field);

        string DisplayString(DateTimeRangeFieldViewModel viewModel);
    }

    public class DateTimeRangeService : IDateTimeRangeService
    {
        private readonly IDateLocalizationServices _dateLocalizationServices;

        public DateTimeRangeService(IDateLocalizationServices dateLocalizationServices)
        {
            _dateLocalizationServices = dateLocalizationServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public DateTimeRangeFieldViewModel BuildViewModel(DateTimeRangeField field)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<DateTimeRangeFieldSettings>();

            var viewModel = new DateTimeRangeFieldViewModel()
            {
                Name = field.DisplayName,
                Settings = settings,
            };
            viewModel.SetDateTimeShown(settings.Display);

            viewModel.DateTimeRanges = field.DateTimeRanges
                    .Select(p =>
                    {
                        var utcDateFrom = !String.IsNullOrWhiteSpace(p.DateFrom)
                            ? DateTime.Parse(p.DateFrom, CultureInfo.InvariantCulture)
                            : new DateTime(1980, 1, 1);
                        var utcTimeFrom = !String.IsNullOrWhiteSpace(p.TimeFrom)
                            ? DateTime.Parse(p.TimeFrom, CultureInfo.InvariantCulture)
                            : new DateTime(1980, 1, 1, 12, 0, 0);
                        var utcDateTimeFrom = new DateTime(
                            utcDateFrom.Year, utcDateFrom.Month, utcDateFrom.Day,
                            utcTimeFrom.Hour, utcTimeFrom.Minute, utcTimeFrom.Second);

                        var utcDateTo = !String.IsNullOrWhiteSpace(p.DateTo)
                            ? DateTime.Parse(p.DateTo, CultureInfo.InvariantCulture)
                            : new DateTime(1980, 1, 1);
                        var utcTimeTo = !String.IsNullOrWhiteSpace(p.TimeTo)
                            ? DateTime.Parse(p.TimeTo, CultureInfo.InvariantCulture)
                            : new DateTime(1980, 1, 1, 12, 0, 0);
                        var utcDateTimeTo = new DateTime(
                            utcDateTo.Year, utcDateTo.Month, utcDateTo.Day,
                            utcTimeTo.Hour, utcTimeTo.Minute, utcTimeTo.Second);


                        var dateTimeRange = new MainBit.Fields.ViewModels.DateTimeRangeEditor()
                        {
                            From = new DateTimeEditor()
                            {
                                Date = !String.IsNullOrWhiteSpace(p.DateFrom)
                                    ? _dateLocalizationServices.ConvertToLocalizedDateString(utcDateTimeFrom)
                                    : string.Empty,
                                Time = !String.IsNullOrWhiteSpace(p.TimeFrom)
                                    ? _dateLocalizationServices.ConvertToLocalizedTimeString(utcDateTimeFrom)
                                    : string.Empty
                            },
                            To = new DateTimeEditor()
                            {
                                Date = !String.IsNullOrWhiteSpace(p.DateTo)
                                    ? _dateLocalizationServices.ConvertToLocalizedDateString(utcDateTimeTo)
                                    : string.Empty,
                                Time = !String.IsNullOrWhiteSpace(p.TimeTo)
                                    ? _dateLocalizationServices.ConvertToLocalizedTimeString(utcDateTimeTo)
                                    : string.Empty
                            },
                        };

                        dateTimeRange.From.ShowDate = viewModel.ShowDateFrom;
                        dateTimeRange.From.ShowTime = viewModel.ShowTimeFrom;
                        dateTimeRange.To.ShowDate = viewModel.ShowDateTo;
                        dateTimeRange.To.ShowTime = viewModel.ShowTimeTo;

                        return dateTimeRange;
                    })
                    .ToList();

            return viewModel;
        }

        public string DisplayString(DateTimeRangeFieldViewModel viewModel)
        {
            if (viewModel.DateTimeRanges == null)
            {
                return string.Empty;
            }

            string[] translatedValues = viewModel.DateTimeRanges.Select(v =>
            {
                var valueFormat = "";
                if (v.From.ShowDate && v.From.ShowTime && !v.To.ShowDate && v.To.ShowTime)
                {
                    valueFormat = "{0} {1}-{3}";
                }
                return string.Format(valueFormat, v.From.Date, v.From.Time, v.To.Date, v.To.Time);

            }).ToArray();
            string separator = T(", ").ToString();
            return string.Join(separator, translatedValues);
        }
    }
}