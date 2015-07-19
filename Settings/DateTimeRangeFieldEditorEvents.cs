using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace MainBit.Fields.Settings {
    public class DateTimeFieldEditorEvents : ContentDefinitionEditorEventsBase {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "DateTimeRangeField") {
                var model = definition.Settings.GetModel<DateTimeRangeFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "DateTimeRangeField")
            {
                yield break;
            }

            var model = new DateTimeRangeFieldSettings();
            if (updateModel.TryUpdateModel(model, "DateTimeRangeFieldSettings", null, null))
            {
                builder.WithSetting("DateTimeRangeFieldSettings.Display", model.Display.ToString());
                builder.WithSetting("DateTimeRangeFieldSettings.Hint", model.Hint);
                builder.WithSetting("DateTimeRangeFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("DateTimeRangeFieldSettings.Multiple", model.Multiple.ToString(CultureInfo.InvariantCulture));

                yield return DefinitionTemplate(model);
            }
        }
    }
}