using System;
namespace MainBit.Fields.Settings
{
    public enum DateTimeRangeFieldDisplays
    {
        TimeRangeOfSpecificDate,
        //DateAndTimeRange,
        //DateRangeOfSpecificTime,
        //TimeRangeOnly,
        //DateRangeOnly,
    }

    public class DateTimeRangeFieldSettings {
        public DateTimeRangeFieldDisplays Display { get; set; }
        public string Hint { get; set; }
        public bool Required { get; set; }
        public bool Multiple { get; set; }
    }
}
