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


namespace WinClip
{
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        #region Persisted Editable Properties
        [DisplayName("Max Clips")]
        [Description("Max size of clip cache.")]
        public int MaxClips { get; set; } = 20;

        [DisplayName("Clip Size")]
        [Description("Size in UI.")]
        public Size ClipSize { get; set; } = new(200, 80);

        [DisplayName("Display Font")]
        [Description("The font to use for plain text clip displays.")]
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

        // Make configurable or calculated?
        [Browsable(false)]
        public int ShortTextLen { get; set; } = 60;
        #endregion
    }
}
