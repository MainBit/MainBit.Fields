using MainBit.Fields.Settings;
using Orchard.Core.Common.ViewModels;
using System.Collections.Generic;

namespace MainBit.Fields.ViewModels {
    public class DateTimeRangeFieldViewModel {
        public string Name { get; set; }
        public bool ShowDateFrom { get; set; }
        public bool ShowTimeFrom { get; set; }
        public bool ShowDateTo { get; set; }
        public bool ShowTimeTo { get; set; }
        public DateTimeRangeFieldSettings Settings { get; set; }
        public IList<DateTimeRangeEditor> DateTimeRanges { get; set; }
        public DateTimeRangeFieldViewModel() {
            DateTimeRanges = new List<DateTimeRangeEditor>();
        }

        public void SetDateTimeShown(DateTimeRangeFieldDisplays display)
        {
            ShowDateFrom = ShowTimeFrom = ShowDateTo = ShowTimeTo = false;
            switch (display)
	        {
		        case DateTimeRangeFieldDisplays.TimeRangeOfSpecificDate:
                    ShowDateFrom = ShowTimeFrom = ShowTimeTo = true;
                    break;
                default:
                    break;
	        }
        }
    }

    public class DateTimeRangeEditor
    {
        public DateTimeEditor From { get; set; }
        public DateTimeEditor To { get; set; }
    }
}