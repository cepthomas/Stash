using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.IconicSelector;


namespace Stash
{
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        #region Persisted Editable Properties
        [DisplayName("Max Chunks")]
        [Description("Max size of cache.")]
        public int MaxChunks { get; set; } = 20;

        [DisplayName("Chunk Size")]
        [Description("Size in UI.")]
        public Size ChunkSize { get; set; } = new(200, 80);

        [DisplayName("Display Font")]
        [Description("The font to use for plain text chunk displays.")]
        [JsonConverter(typeof(JsonFontConverter))]
        public Font DisplayFont { get; set; } = new("Consolas", 12, FontStyle.Regular, GraphicsUnit.Point, 0);

        [DisplayName("Marker Color")]
        [Description("The color used for markers.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color MarkerColor { get; set; } = Color.Blue;

        [DisplayName("File Log Level")]
        [Description("Log level for file write.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel FileLogLevel { get; set; } = LogLevel.Trace;

        [DisplayName("File Log Level")]
        [Description("Log level for UI notification.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel NotifLogLevel { get; set; } = LogLevel.Debug;
        #endregion

        #region Persisted Non-editable Properties
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // [Browsable(false)]
        // public List<string> Targets { get; set; } = [];
        #endregion
    }
}
