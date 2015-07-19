using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using System.Collections.Generic;
using System.Globalization;

namespace MainBit.Fields.Fields {
    public class DateTimeRangeField : ContentField {

        private const char DateTimeRangeSeparator = ';';
        private const char DateTimeSeparator = '-';

        public string Value
        {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value ?? String.Empty); }
        }

        private List<DateTimeRange> _dateTimeRanges = null;
        public List<DateTimeRange> DateTimeRanges
        {
            get
            {
                if (_dateTimeRanges != null)
                {
                    return _dateTimeRanges;
                }

                if (string.IsNullOrWhiteSpace(Value))
                {
                    _dateTimeRanges = new List<DateTimeRange>();
                    return _dateTimeRanges;
                }
                
                _dateTimeRanges = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTimeRange>>(Value);
                return _dateTimeRanges;
            }

            set
            {
                Value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                _dateTimeRanges = value;
            }
        }
    }

    public class DateTimeRange
    {
        public string DateFrom { get; set; }
        public string TimeFrom { get; set; }
        public string DateTo { get; set; }
        public string TimeTo { get; set; }
    }
}
