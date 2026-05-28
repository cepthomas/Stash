using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace WinClip
{
    /// <summary>Abstract base class for all clip types.</summary>
    [Serializable]
    public abstract class ClipBase
    {
        #region Properties

        /// <summary>For display.</summary>
        public Bitmap Thumbnail { get; protected set; }

        /// <summary>Common - set by client.</summary>
        public static Size ClipSize
        {
            get { return _clipSize; }
            set { _clipSize = value; _shortTextLen = _clipSize.Width * _clipSize.Height / 32; }
        }
        #endregion

        #region Fields
        /// <summary>Windows data type.</summary>
        protected string _typeName = "???";

        /// <summary>Backing field.</summary>
        static Size _clipSize = new();

        /// <summary>Truncated text to show.</summary>
        protected static int _shortTextLen = 50;
        #endregion

        /// <summary>Constructor</summary>
        protected ClipBase()
        {
            // Default thumbnail.
            Thumbnail = new(ClipSize.Width, ClipSize.Height);
        }

        #region Abstract functions
        /// <summary>
        /// Convert to clipboard format.
        /// </summary>
        /// <returns></returns>
        public abstract IDataObject? ToData();
        #endregion
    }

    /// <summary>Plain text.</summary>
    [Serializable]
    public class PlainTextClip : ClipBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public string Content { get; private set; }
        #endregion

        #region Fields

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public PlainTextClip(IDataObject data)
        {
            _typeName = "System.String";
            var sdata = data.GetData(_typeName);
            Content = (string)sdata!;
            var text = Content.Left(_shortTextLen);

            // Make a bitmap from text.
            using RichTextBox rtb = new()
            {
                BorderStyle = BorderStyle.None,
                WordWrap = false,
                Text = text,
                Size = ClipSize,
                //Font = DisplayFont,
                ScrollBars = RichTextBoxScrollBars.None
            };
            Thumbnail = new Bitmap(rtb.Width, rtb.Height);
            rtb.DrawToBitmap(Thumbnail, new Rectangle(0, 0, rtb.Width, rtb.Height));
        }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            var dobj = new DataObject(Content);
            return dobj;
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"PlainTextClip:[{Content.Left(_shortTextLen)}]";
        }
    }

    /// <summary>RTF text.</summary>
    [Serializable]
    public class RtfTextClip : ClipBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public string Content { get; private set; }
        #endregion

        #region Fields
        public string _shortText;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public RtfTextClip(IDataObject data)
        {
            _typeName = "Rich Text Format";
            var sdata = data.GetData(_typeName);
            Content = (string)sdata!;

            using RichTextBox rtb = new()
            {
                Rtf = Content,
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                Size = ClipSize,
                ScrollBars = RichTextBoxScrollBars.None
            };
            _shortText = rtb.Text.Left(_shortTextLen);
            Thumbnail = new Bitmap(rtb.Width, rtb.Height);
            rtb.DrawToBitmap(Thumbnail, new Rectangle(0, 0, rtb.Width, rtb.Height));
        }



        // /// <summary>
        // /// Extract plain text from rtf.
        // /// </summary>
        // /// <param name="rtf"></param>
        // /// <returns></returns>
        // protected string RtfToText(string rtf)
        // {
        //     using RichTextBox rtb = new()
        //     {
        //         Rtf = rtf,
        //         WordWrap = false,
        //     };
        //     return rtb.Text;
        // }

        // /// <summary>
        // /// Make a bitmap from text.
        // /// </summary>
        // /// <param name="rtf"></param>
        // protected void RenderRtf(string rtf)
        // {
        //     using RichTextBox rtb = new()
        //     {
        //         BorderStyle = BorderStyle.None,
        //         WordWrap = false,
        //         Rtf = rtf,
        //         Size = ClipSize,
        //         ScrollBars = RichTextBoxScrollBars.None
        //     };
        //     Thumbnail = new Bitmap(rtb.Width, rtb.Height);
        //     rtb.DrawToBitmap(Thumbnail, new Rectangle(0, 0, rtb.Width, rtb.Height));
        // }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            return new DataObject(Content);
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"RtfTextClip:[{_shortText}]";
        }
    }

    /// <summary>Image.</summary>
    [Serializable]
    public class ImageClip : ClipBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public Bitmap Content { get; private set; }
        #endregion

        #region Fields

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public ImageClip(IDataObject data)
        {
            _typeName = "System.Drawing.Bitmap";
            var idata = data.GetData(_typeName);
            Content = (Bitmap)idata!;
            // Make a thumbnail scaled to available real estate.
            float ratio = (float)ClipSize.Height / Content.Height;
            int tnWidth = (int)(Content.Width * ratio);
            int tnHeight = ClipSize.Height;
            Thumbnail = MiscUtils.ResizeBitmap(Content, tnWidth, tnHeight);
        }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            return new DataObject(Content);
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"ImageClip:[{Content.Size}]";
        }
    }

    /// <summary>Could be unknown/empty/unsupported.</summary>
    [Serializable]
    public class DefaultClip : ClipBase
    {
        #region Fields
        readonly IDataObject? _data = null;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public DefaultClip(IDataObject? data)
        {
            _data = data;
            //var formats = data.GetFormats();

            // Default thumbnail.
            Thumbnail = new(ClipSize.Width, ClipSize.Height);
            using Graphics gr = Graphics.FromImage(Thumbnail);
            gr.Clear(Color.LightSalmon);
            gr.DrawString($"????", Font, Brushes.Black, 2, 2);
        }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            return _data;
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"DefaultClip:[{_data}]";
        }
    }
}
