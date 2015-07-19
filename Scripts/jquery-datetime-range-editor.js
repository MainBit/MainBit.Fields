ko.bindingHandlers.dateTimeRangeItemName = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.name = viewModel.root.itemNamePrefix + '[' + viewModel.index + '].' + valueAccessor();
    }
};
ko.bindingHandlers.dateTimeRangeItemId = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.id = viewModel.root.itemIdPrefix + '_' + viewModel.index + '__' + valueAccessor();
    }
};
ko.bindingHandlers.dateTimeRangeItemIdFor = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.htmlFor = viewModel.root.itemIdPrefix + '_' + viewModel.index + '__' + valueAccessor();
    }
}
ko.bindingHandlers.required = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.mainbitRequired = false;
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        if (viewModel.isRequired()) {
            if (!element.mainbitRequired) {
                element.mainbitRequired = true;
                element.setAttribute("required", "required");
                if (value && value.setFieldRequired) {
                    value.setFieldRequired(element);
                }
            }
        }
        else {
            if (element.mainbitRequired) {
                element.mainbitRequired = false;
                element.removeAttribute("required");
                if (value && value.setFielNotdRequired) {
                    value.setFielNotdRequired(element);
                }
            }
        }
    }
};

var DateTimeRange = function (root, index, dateFrom, timeFrom, dateTo, timeTo) {

    var self = this;

    self.index = index;
    self.dateFrom = ko.observable(dateFrom || '');
    self.timeFrom = ko.observable(timeFrom || '');
    self.dateTo = ko.observable(dateTo || '');
    self.timeTo = ko.observable(timeTo || '');

    self.root = root;
    self.isLast = function () {
        return self == self.root.getLastItem();
    };

    self.isAllSet = function () {
        if ((!root.showDateFrom || self.dateFrom() != '') && (!root.showTimeFrom || self.timeFrom() != '')
            && (!root.showDateTo || self.dateTo() != '') && (!root.showTimeTo || self.timeTo() != ''))
        {
            return true;
        }
        return false;
    }

    self.isNotingSet = function () {
        if ((!root.showDateFrom || (root.showDateFrom && self.dateFrom() == ''))
            && (!root.showTimeFrom || (root.showTimeFrom && self.timeFrom() == ''))
            && (!root.showDateTo || (root.showDateTo && self.dateTo() == ''))
            && (!root.showTimeTo || (root.showTimeTo && self.timeTo() == ''))) {
            return true;
        }
        return false;
    }

    self.isPartialSet = function () {
        return !self.isAllSet() && !self.isNotingSet();
    }

    self.isRequired = function () {
        if (self.root.items().length == 1) {
            return true;
        }
        return !self.isLast() || !self.isNotingSet();
    }
}

var DateTimeRangeViewModel = function (containerId, vm) {

    var self = this;

    self.itemNamePrefix = vm.itemNamePrefix;
    self.itemIdPrefix = vm.itemIdPrefix;
    self.showDateFrom = vm.showDateFrom;
    self.showTimeFrom = vm.showTimeFrom;
    self.showDateTo = vm.showDateTo;
    self.showTimeTo = vm.showTimeTo;
    self.multiple = vm.multiple;

    self.maxIndex = -1;

    self.getLastItem = function() {
        var items = self.items();
        return items[items.length - 1];
    }
    self.removeItem = function (item) {
        self.items.remove(item);
    };

    self.addItem = function () {
        self.items.push(new DateTimeRange(self, ++self.maxIndex));
    };

    self.onChangeItem = function (item) {
        if (item.isLast() && item.isAllSet()) {
            self.addItem();
        }

        if (item.isLast() && !item.isNotingSet()) {
            var from = $(document.getElementById(containerId)).closest('form');
            from.trigger( "destroy.vv" );
        }
    }

    var dateTimeRanges = []
    for (var i = 0; i < vm.items.length; i++) {
        var item = vm.items[i];
        var dateTimeRange = new DateTimeRange(self, ++self.maxIndex, item.dateFrom, item.timeFrom, item.dateTo, item.timeTo);
        dateTimeRanges.push(dateTimeRange);
    }
    self.items = ko.observableArray(dateTimeRanges);
    if (self.items().length == 0 || (self.multiple && self.getLastItem().isAllSet())) {
        self.addItem();
    }

    self.initOrchardDateTimePickers = function (element, index, data) {
        var picker = "<div {popup:start} id='ui-datepicker-div'{popup:end} class='ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all{inline:start} ui-datepicker-inline{inline:end}'><div class='ui-datepicker-header ui-widget-header ui-helper-clearfix ui-corner-all'>{link:prev}{link:today}{link:next}</div>{months}{popup:start}{popup:end}<div class='ui-helper-clearfix'></div></div>";
        var month = "<div class='ui-datepicker-group'><div class='ui-datepicker-month ui-helper-clearfix'>{monthHeader:MM yyyy}</div><table class='ui-datepicker-calendar'><thead>{weekHeader}</thead><tbody>{weeks}</tbody></table></div>";

        var that = $(element);
        that.find('.date-edit input').calendarsPicker({
            showAnim: "",
            renderer: $.extend({}, $.calendars.picker.themeRollerRenderer, {
                picker: picker,
                month: month
            }),
            onSelect: function () {
                $(this).change();
            }
        });
        that.find('.time-edit input').timeEntry();
    }

    self.initMainBitDateTimePickers = function (element, index, data) {
        var that = $(element);
        that.find('.date-edit input').datetimepicker({
            lang: 'ru',
            timepicker: false,
            format: 'd.m.Y',
            closeOnDateSelect: true,
            defaultSelect: false,
            validateOnBlur: true,
            allowBlank: true,
            dayOfWeekStart: 1,
            scrollMonth: false,
            i18n: { ru: { dayOfWeek: ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"] } },
            onGenerate: function () {
                $(this).find('.xdsoft_date.xdsoft_weekend').addClass('xdsoft_disabled');
                var monthPicker = $(this).find('.xdsoft_mounthpicker');
                monthPicker.find('.xdsoft_month').off('.xdsoft');
                monthPicker.find('.xdsoft_prev, .xdsoft_next')
                    .addClass("button_type_svg")
                    .html('<svg><use xlink:href="#svg_date_next-month"></use></svg>');
                monthPicker.find('.xdsoft_today_button')
                    .addClass("button_type_svg")
                    .html('<svg><use xlink:href="#svg_date_current"></use></svg>');
            },
        });

        that.find('.time-edit input').datetimepicker({
            lang: 'ru',
            datepicker: false,
            format: 'H:i',
            closeOnDateSelect: true,
            defaultSelect: false,
            validateOnBlur: true,
            allowBlank: true,
            allowTimes: ['0:00', '1:00', '2:00', '3:00', '4:00', '5:00', '6:00', '7:00', '8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00', '21:00', '22:00', '23:00', '24:00'],
            onGenerate: function () {
                $(this).find('.xdsoft_timepicker').find('.xdsoft_prev, .xdsoft_next')
                    .addClass("button_type_svg")
                    .html('<svg><use xlink:href="#svg_time_scroll-up"></use></svg>');
            },
        });
    }

    ko.applyBindings(this, document.getElementById(containerId));
};

