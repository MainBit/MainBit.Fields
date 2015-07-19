using System;
using System.Linq;
using System.Globalization;
using System.Xml;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Core.Common.ViewModels;
using Orchard;
using MainBit.Fields.Settings;
using MainBit.Fields.Fields;
using MainBit.Fields.ViewModels;
using System.Collections.Generic;

namespace MainBit.Fields.Drivers {
    [UsedImplicitly]
    public class DateTimeRangeFieldDriver : ContentFieldDriver<DateTimeRangeField> {
        private const string TemplateName = "Fields/DateTimeRange.Edit"; // EditorTemplates/Fields/DateTimeRange.Edit.cshtml

        public DateTimeRangeFieldDriver(IOrchardServices services, IDateServices dateServices)
        {
            Services = services;
            DateServices = dateServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public IDateServices DateServices { get; set; }
        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, DateTimeRangeField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_DateTimeRange", // this is just a key in the Shape Table
                GetDifferentiator(field, part),
                () => {
                    var viewModel = BuildViewModel(field);
                    return shapeHelper.Fields_DateTimeRange( // this is the actual Shape which will be resolved (Fields/DateTimeRange.cshtml)
                        Model: viewModel);
                }
            );
        }

        protected override DriverResult Editor(ContentPart part, DateTimeRangeField field, dynamic shapeHelper) {

            var viewModel = BuildViewModel(field);
            return ContentShape("Fields_DateTimeRange_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, DateTimeRangeField field, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new DateTimeRangeFieldViewModel();

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {

                var settings = field.PartFieldDefinition.Settings.GetModel<DateTimeRangeFieldSettings>();
                viewModel.SetDateTimeShown(settings.Display);

                var newDateTimeRanges = new List<Fields.DateTimeRange>();
                try {
                    for (var i = 0; i < viewModel.DateTimeRanges.Count; i++) 
                    {
                        var dateTimeRange = viewModel.DateTimeRanges[i];

                        if (settings.Multiple && i + 1 == viewModel.DateTimeRanges.Count
                            && string.IsNullOrEmpty(dateTimeRange.From.Date) && string.IsNullOrEmpty(dateTimeRange.From.Time)
                            && string.IsNullOrEmpty(dateTimeRange.To.Date) && string.IsNullOrEmpty(dateTimeRange.To.Time))
                        {
                            continue;
                        }

                        var newDateTimeRange = new Fields.DateTimeRange();
                        var utcDateTimeForm = DateServices.ConvertFromLocalString(dateTimeRange.From.Date, dateTimeRange.From.Time);
                        var utcDateTimeTo = DateServices.ConvertFromLocalString(dateTimeRange.To.Date, dateTimeRange.To.Time);

                        if (utcDateTimeForm.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(dateTimeRange.From.Date))
                            {
                                newDateTimeRange.DateFrom = utcDateTimeForm.Value.ToString("d", CultureInfo.InvariantCulture);
                            }
                            if (!string.IsNullOrWhiteSpace(dateTimeRange.From.Time))
                            {
                                newDateTimeRange.TimeFrom = utcDateTimeForm.Value.ToString("t", CultureInfo.InvariantCulture);
                            }
                        }
                        if (utcDateTimeTo.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(dateTimeRange.To.Date))
                            {
                                newDateTimeRange.DateTo = utcDateTimeTo.Value.ToString("d", CultureInfo.InvariantCulture);
                            }
                            if (!string.IsNullOrWhiteSpace(dateTimeRange.To.Time))
                            {
                                newDateTimeRange.TimeTo = utcDateTimeTo.Value.ToString("t", CultureInfo.InvariantCulture);
                            }
                        }

                        

                        if ((viewModel.ShowDateFrom && String.IsNullOrWhiteSpace(newDateTimeRange.DateFrom))
                            || (viewModel.ShowTimeFrom && String.IsNullOrWhiteSpace(newDateTimeRange.TimeFrom))
                            || (viewModel.ShowDateTo && String.IsNullOrWhiteSpace(newDateTimeRange.DateTo))
                            || (viewModel.ShowTimeTo && String.IsNullOrWhiteSpace(newDateTimeRange.TimeTo)))
                        {
                            updater.AddModelError(GetPrefix(field, part), T("{0} is required that all fields be completed.", field.DisplayName));
                        }

                        newDateTimeRanges.Add(newDateTimeRange);
                    }
                }
                catch (FormatException)
                {
                    updater.AddModelError(GetPrefix(field, part), T("{0} could not be parsed as a valid date and time.", field.DisplayName));
                }
                
                if (settings.Required && newDateTimeRanges.Count == 0)
                {
                    updater.AddModelError(GetPrefix(field, part), T("{0} is required.", field.DisplayName));
                }
                else {
                    field.DateTimeRanges = newDateTimeRanges;
                }
            }

            return Editor(part, field, shapeHelper);
        }

        private DateTimeRangeFieldViewModel BuildViewModel(DateTimeRangeField field)
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
                                    ? DateServices.ConvertToLocalDateString(utcDateTimeFrom, String.Empty)
                                    : string.Empty,
                                Time = !String.IsNullOrWhiteSpace(p.TimeFrom)
                                    ? DateServices.ConvertToLocalTimeString(utcDateTimeFrom, String.Empty)
                                    : string.Empty
                            },
                            To = new DateTimeEditor()
                            {
                                Date = !String.IsNullOrWhiteSpace(p.DateTo)
                                    ? DateServices.ConvertToLocalDateString(utcDateTimeTo, String.Empty)
                                    : string.Empty,
                                Time = !String.IsNullOrWhiteSpace(p.TimeTo)
                                    ? DateServices.ConvertToLocalTimeString(utcDateTimeTo, String.Empty)
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
    }
}
