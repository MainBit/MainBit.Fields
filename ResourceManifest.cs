using Orchard.UI.Resources;

namespace Orchard.jQuery {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("Knockout").SetUrl("knockout-3.2.0.js", "knockout-3.2.0.js");
            
            // Plagins
            manifest.DefineScript("jQueryPlagin_DateTimePicker").SetUrl("jquery.datetimepicker.js", "jquery.datetimepicker.min.js").SetDependencies("jQuery");
            
            // jQuery Date/Time Editor Enhancements
            manifest.DefineScript("jQueryDateTimeRangeEditor").SetUrl("jquery-datetime-range-editor.js").SetDependencies("jQueryPlagin_DateTimePicker", "Knockout");
            manifest.DefineStyle("jQueryDateTimeRangeEditor").SetUrl("jquery-datetime-range-editor.css");
        }
    }
}
