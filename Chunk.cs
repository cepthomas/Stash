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


namespace Stash
{
    /// <summary>Abstract base class for all chunk types.</summary>
    [Serializable]
    public abstract class ChunkBase
    {
        #region Properties
        /// <summary>For display.</summary>
        public Bitmap Thumbnail { get; protected set; }

        /// <summary>Common - set by client.</summary>
        public static Size ChunkSize
        {
            get { return _chunkSize; }
            set { _chunkSize = value; _shortTextLen = _chunkSize.Width * _chunkSize.Height / 32; }
        }
        #endregion

        #region Fields
        /// <summary>Backing field.</summary>
        static Size _chunkSize = new();

        /// <summary>Truncated text to show.</summary>
        protected static int _shortTextLen = 50;
        #endregion

        /// <summary>Constructor</summary>
        protected ChunkBase()
        {
            // Default thumbnail.
            Thumbnail = new(ChunkSize.Width, ChunkSize.Height);
        }

        #region Abstract members
        /// <summary>
        /// Convert to clipboard format.
        /// </summary>
        /// <returns></returns>
        public abstract IDataObject? ToData();
        #endregion
    }

    /// <summary>Plain text.</summary>
    [Serializable]
    public class PlainTextChunk : ChunkBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public string Content { get; private set; }
        #endregion

        #region Fields
        /// <summary>Windows data type.</summary>
        public static string TypeName = "System.String";
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public PlainTextChunk(IDataObject data)
        {
            var sdata = data.GetData(TypeName);
            Content = (string)sdata!;
            var text = Content.Left(_shortTextLen);

            // Make a bitmap from text.
            using RichTextBox rtb = new()
            {
                BorderStyle = BorderStyle.None,
                WordWrap = false,
                Text = text,
                Size = ChunkSize,
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
            return $"PlainTextChunk:[{Content.Left(_shortTextLen)}]";
        }
    }

    /// <summary>RTF text.</summary>
    [Serializable]
    public class RtfTextChunk : ChunkBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public string Content { get; private set; }
        #endregion

        #region Fields
        /// <summary>Windows data type.</summary>
        public static string TypeName = "Rich Text Format";

        public string _shortText;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public RtfTextChunk(IDataObject data)
        {
            var sdata = data.GetData(TypeName);
            Content = (string)sdata!;

            using RichTextBox rtb = new()
            {
                Rtf = Content,
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                Size = ChunkSize,
                ScrollBars = RichTextBoxScrollBars.None
            };
            _shortText = rtb.Text.Left(_shortTextLen);
            Thumbnail = new Bitmap(rtb.Width, rtb.Height);
            rtb.DrawToBitmap(Thumbnail, new Rectangle(0, 0, rtb.Width, rtb.Height));
        }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            return new DataObject(Content);
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"RtfTextChunk:[{_shortText}]";
        }
    }

    /// <summary>Image.</summary>
    [Serializable]
    public class ImageChunk : ChunkBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public Bitmap Content { get; private set; }
        #endregion

        #region Fields
        /// <summary>Windows data type.</summary>
        public static string TypeName = "System.Drawing.Bitmap";
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public ImageChunk(IDataObject data)
        {
            var idata = data.GetData(TypeName);
            Content = (Bitmap)idata!;
            // Make a thumbnail scaled to available real estate.
            float ratio = (float)ChunkSize.Height / Content.Height;
            int tnWidth = (int)(Content.Width * ratio);
            int tnHeight = ChunkSize.Height;
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
            return $"ImageChunk:[{Content.Size}]";
        }
    }

    /// <summary>Could be unknown/empty/unsupported.</summary>
    [Serializable]
    public class DefaultChunk : ChunkBase
    {
        #region Properties
        /// <summary>Actual content.</summary>
        public string Content { get; private set; }
        #endregion

        #region Fields
        readonly IDataObject _data;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public DefaultChunk(IDataObject data)
        {
            _data = data;
            Content = string.Join(Environment.NewLine, data.GetFormats());

            // Default thumbnail.
            Thumbnail = new(ChunkSize.Width, ChunkSize.Height);
            using Graphics gr = Graphics.FromImage(Thumbnail);
            gr.Clear(Color.LightSalmon);
            //gr.DrawString($"????", this.Font, Brushes.Black, 2, 2);
        }

        /// <inheritdoc />
        public override IDataObject? ToData()
        {
            return _data;
        }

        /// <summary>For viewing pleasure.</summary>
        public override string ToString()
        {
            return $"DefaultChunk:[{_data}]";
        }
    }
}
