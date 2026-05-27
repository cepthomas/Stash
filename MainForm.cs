using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.IconicSelector;
using W32 = Ephemera.Win32.Internals;
using WM = Ephemera.Win32.WindowManagement;


// TODO1 - Frequently throws System.ExecutionEngineException. Previously indicated
// an unspecified fatal error in the runtime. MS says: The runtime no longer raises
// this exception so this type is obsolete.
// Other gleanings:
//   - Calling the Environment.FailFast method to terminate execution of an application running in the
//   Visual Studio debugger throws an ExecutionEngineException and automatically triggers the
//   fatalExecutionEngineError managed debugging assistant (MDA).
//   - Try disable hot reload/edit & continue.


namespace WinClip
{
    /// <summary>
    /// - Handles all interactions at the Clipboard.XXX() API level.
    /// - Hooks keyboard to intercept magic paste key.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Types
        record struct WindowInfo(IntPtr Hwnd, int ProcessId, string ProcessName, string Title, bool IsVisible)
        {
            public override readonly string ToString() { return $"hwnd:0X{Hwnd:X8} proc:{ProcessId:X8}[{ProcessName}] title:[{Title}] vis:{IsVisible}"; }
        };
        #endregion

        #region Fields
        /// <summary>App logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("APP");

        /// <summary>The settings.</summary>
        readonly UserSettings _settings;

        ///// <summary>Save what happens.</summary>
        //readonly List<string> _log = [];

        // /// <summary>All clips in the collection.</summary>
        // readonly List<ClipDisplay> _clips = new();

        /// <summary>Where to paste.</summary>
        IntPtr _pasteWin = IntPtr.Zero;

        /// <summary>Handle to the window event hook.</summary>
        readonly IntPtr _winEventHook = IntPtr.Zero;

        ///// <summary>Manage resources.</summary>
        //bool _disposed;

        /// <summary>Dev.</summary>
        Dev _dev = new(); // TODO1

        #region Workarounds 
        // win11 bug
        Size _lastBmpSize = new();
        DateTime _lastBmpTime = DateTime.Now;
        // multiple identical messages
        int _lastClipboardSeq = -1;
        #endregion
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        public MainForm(string[] args)
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // Load settings first before initializing.
            string appDir = MiscUtils.GetAppDataDir("WinClip", "Ephemera");
            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));

            ClipBase.ClipSize = _settings.ClipSize;
            ClipBase.ShortTextLen = _settings.ShortTextLen;

            // Init logging.
            string logFileName = Path.Combine(appDir, "log.txt");
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogMessage += (object? sender, LogMessageEventArgs e) => { Tell(e.Message); };
            LogManager.Run(logFileName, 50000);

            Text = $"WinClip {MiscUtils.GetVersionString()}";

            // Init selector configuration.
            var config = new Config()
            {
                AllowExternalSource = false,
                IndicatorColor = _settings.MarkerColor,
                Spacing = 10,
                Pad = 8,
                Mode = OpMode.Click,
                Style = SelectorStyle.Icon,
                NumColumns = 1,
                ImageSize = _settings.ClipSize,
            };
            selector.Init(config);

            // Hook selector events.
            selector.Click += Selector_Click;

            // Selector menu. TODO1 need a way to delete clip - probably should be in sel lib
            selector.ContextMenuStrip = new();
            selector.ContextMenuStrip.Items.Add("Remove");
            selector.ContextMenuStrip.ItemClicked += Menu_ItemClicked;

            // Init the data. TODO1 persisted?
            //_settings.Targets.ForEach(item => selector.AddResourceItem(item));

            // Size and location. TODO1 probably user option? always/popup/?

            FormBorderStyle = FormBorderStyle.SizableToolWindow;// FixedToolWindow;
            StartPosition = FormStartPosition.Manual;
            Size = new(selector.GetTotalArea().Width + SystemInformation.VerticalScrollBarWidth, 600);
            WindowState = FormWindowState.Normal;
            //WindowState = FormWindowState.Minimized;
            var pos = Cursor.Position;
            Location = new Point(200, 200);

            ///// System hooks.
            // Listen for window changes.
            _winEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                WindowEventCallback, 0, 0, WINEVENT_OUTOFCONTEXT); // | WINEVENT_SKIPOWNPROCESS);
            if (_winEventHook == IntPtr.Zero) { throw new Win32Exception(Marshal.GetLastWin32Error()); }

            // Listen for clipboard changes.
            var res = AddClipboardFormatListener(Handle);
        }

        /// <summary>
        /// User wants to do something.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Menu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            selector.ContextMenuStrip!.Close();

            switch (e.ClickedItem!.Text)
            {
                case "Remove": // TODO1 ??
                    //var sels = selector.GetSelectedItems();
                    //sels.ForEach(sel => selector.RemoveItem(sel));
                    break;
            }
        }

        /// <summary>
        /// Clean up on shutdown.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogManager.Stop();

            // Save user settings.
            _settings.FormGeometry = new()
            {
                X = Location.X,
                Y = Location.Y,
                Width = Width,
                Height = Height
            };

            _settings.Save();

            base.OnFormClosing(e);
        }

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                selector?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Selector interaction
        /// <summary>
        /// A clip was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Selector_Click(object? sender, ClickEventArgs e)
        {
            if (e.ClickedItem is not null)
            {
                // Item click.
                //_logger.Info($"Selection -> [{e.ClickedItem.Caption}] [{e.ClickedItem.Value}]");

                if (e.ClickedItem.DataType == ItemDataType.User)
                {
                    // Push into sys clipboard which will move it to the top.
                    switch (e.ClickedItem.Value)
                    {
                        case PlainTextClip pcltxt:
                            Clipboard.SetData(PlainTextClip.TYPE_NAME, pcltxt.Content);
                            break;

                        case RtfTextClip pclrtf:
                            Clipboard.SetData(RtfTextClip.TYPE_NAME, pclrtf.Content);
                            break;

                        case ImageClip climg:
                            Clipboard.SetData(ImageClip.TYPE_NAME, climg.Content);
                            break;

                        default:
                            // Ignore the impossible.
                            break;
                    }

                    // Send paste to the last window that was foreground, since WinClip is now fg.
                    // Win11 does not allow direct setting of ForegroundWindow. This is now the way:
                    // https://stackoverflow.com/questions/62966320/setforegroundwindow-not-setting-focus
                    var fg = GetWindowInfo(_pasteWin);
                    _logger.Debug($"Paste to win:{_pasteWin:X8} proc:{fg.ProcessId:X8}[{fg.ProcessName}]");
                    Microsoft.VisualBasic.Interaction.AppActivate(fg.ProcessId);
                    SendKeys.Send("^{V}");
                }
            }
        }



        // /// <summary>
        // /// A clip was clicked.
        // /// </summary>
        // /// <param name="clipd">Sender</param>
        // /// <param name="single">Single or double click</param>
        // /// <param name="button">L or R button</param>
        // void ClipClick(ClipDisplay? clipd, bool single, MouseButtons button)
        // {
        //     if (clipd is null)
        //     {
        //         return;
        //     }

        //     // Remove UI from list.
        //     Controls.Remove(clipd);
        //     _clips.Remove(clipd);

        //     // Push into sys clipboard which will move it to the top.
        //     switch (clipd.Clip)
        //     {
        //         case PlainTextClip pcltxt:
        //             Clipboard.SetData(PlainTextClip.TYPE_NAME, pcltxt.Content);
        //             break;

        //         case RtfTextClip pclrtf:
        //             Clipboard.SetData(RtfTextClip.TYPE_NAME, pclrtf.Content);
        //             break;

        //         case ImageClip climg:
        //             Clipboard.SetData(ImageClip.TYPE_NAME, climg.Content);
        //             break;

        //         default:
        //             // Ignore the impossible.
        //             break;
        //     }

        //     // Send paste to the last window that was foreground, since WinClip is now fg.
        //     // Win11 does not allow direct setting of ForegroundWindow. This is now the way:
        //     // https://stackoverflow.com/questions/62966320/setforegroundwindow-not-setting-focus
        //     var fg = GetWindowInfo(_pasteWin);
        //     _logger.Debug($"Paste to win:{_pasteWin:X8} proc:{fg.ProcessId:X8}[{fg.ProcessName}]");
        //     Microsoft.VisualBasic.Interaction.AppActivate(fg.ProcessId);
        //     SendKeys.Send("^{V}");

        //     Invalidate();
        // }





        #endregion




        /// <summary>
        /// Handle window messages.
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case W32.WM_CLIPBOARDUPDATE: // clipboard contents have changed
                    //_logger.Debug($"WM_CLIPBOARDUPDATE suppress:{_suppressClipboardUpdate}");
                    DoClipboardUpdate(msg);
                    msg.Result = -1; // means handled?
                    break;

                case W32.WM_HOTKEY_MESSAGE_ID:
                    DoHotkey(msg);
                    break;

                default:
                    // Ignore, pass along.
                    base.WndProc(ref msg);
                    break;
            }
        }

        /// <summary>
        /// Handle external clip copy ops.
        /// </summary>
        /// <param name="msg"></param>
        void DoClipboardUpdate(Message msg)
        {
            // It's possible to have contention for the clipboard so retry is ok.
            int retries = 5;

            while (retries > 0)
            {
                try
                {
                    IDataObject? dobj = Clipboard.GetDataObject();
                    // var tst = DateTime.Now.ToString("HH':'mm':'ss.fff");

                    // Sometimes we get multiples of the same message. Ignore subsequent of the same seq num.
                    var seq = GetClipboardSequenceNumber();
                    if (seq == _lastClipboardSeq)
                    {
                        return;
                    }
                    _lastClipboardSeq = (int)seq;

                    if (dobj is not null)
                    {
                        // Determine data type - only interested in text and images.
                        ClipBase? clip = null;

                        var fmts = dobj.GetFormats();
                        _dev.Tell($"Copy op: {string.Join("|", fmts)}");
                        if (fmts.Contains(ImageClip.TYPE_NAME))
                        {
                            // Hacks to work around win11 bug in KB5079473 that causes system Print Screen to generate
                            // more than one message. This is a crude way to protect from that until MS fixes the issue.
                            // https://learn.microsoft.com/en-us/answers/questions/5593390/windows-11-25h2-snipping-tool-print-screen-saves-t
                            // https://learn.microsoft.com/en-us/answers/questions/5831588/there-are-two-copies-of-the-screenshot-in-the-clip
                            TimeSpan ts = DateTime.Now - _lastBmpTime;
                            // _logger.Debug($"ts:{ts}");
                            int cutoff = 250; // measured is max 60 msec

                            var img = dobj.GetData(ImageClip.TYPE_NAME);
                            var bmp = img as Bitmap;

                            if (((DateTime.Now - _lastBmpTime).TotalMilliseconds > cutoff) || bmp.Size != _lastBmpSize)
                            {
                                // Not the same so assume valid.

                                //if(_fit) // TODO1 probably should be in sel or other lib
                                //{
                                //    float ratio = (float)_itemdSize.Height / bmp.Height;
                                //    int tnWidth = (int)(bmp.Width * ratio);
                                //    int tnHeight = (int)(bmp.Height * ratio);
                                //    var bmpt = MiscUtils.ResizeBitmap(bmp, tnWidth, tnHeight);
                                //    bmp = bmpt.Clone(new(0, 0, _itemdSize.Width, _itemdSize.Height), PixelFormat.Format32bppArgb);
                                //}


                // case SelectorStyle.FitHeight:
                //     {
                //         float ratio = (float)_itemdSize.Height / bmp.Height;
                //         int tnWidth = (int)(bmp.Width * ratio);
                //         int tnHeight = (int)(bmp.Height * ratio);
                //         var bmpt = MiscUtils.ResizeBitmap(bmp, tnWidth, tnHeight);
                //         bmp = bmpt.Clone(new(0, 0, _itemdSize.Width, _itemdSize.Height), PixelFormat.Format32bppArgb);
                //     }
                //     break;

                // case SelectorStyle.FitWidth:
                //     {
                //         float ratio = (float)_itemdSize.Width / bmp.Width;
                //         int tnHeight = (int)(bmp.Height * ratio);
                //         int tnWidth = (int)(bmp.Width * ratio);
                //         var bmpt = MiscUtils.ResizeBitmap(bmp, tnWidth, tnHeight);
                //         bmp = bmpt.Clone(new(0, 0, _itemdSize.Width, _itemdSize.Height), PixelFormat.Format32bppArgb);
                //     }
                //     break;






                                clip = new ImageClip(dobj);
                            }
                            //else a suspect, wait

                            // Reset state.
                            _lastBmpTime = DateTime.Now;
                            _lastBmpSize = bmp.Size;
                        }
                        else if (fmts.Contains(RtfTextClip.TYPE_NAME))
                        {
                            clip = new RtfTextClip(dobj);
                        }
                        else if (fmts.Contains(PlainTextClip.TYPE_NAME))
                        {
                            clip = new PlainTextClip(dobj);
                        }
                        // else ignore

                        if (clip != null)
                        {
                            // Show it.
                            AddClip(clip);
                            //_logger.Debug($"New clip:{clip}");
                            Invalidate();
                        }

                        retries = 0; // done
                    }
                    else
                    {
                        // _logger.Warn($"GetDataObject() is null");
                        retries--;
                        Thread.Sleep(50);
                    }
                }
                catch (ExternalException ex)
                {
                    _logger.Warn($"WM_DRAWCLIPBOARD ExternalException:{ex}");
                    retries--;
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Other exception:{ex}");
                    retries = 0;
                }
            }
        }

        /// <summary>
        /// Handle hotkey messages.
        /// </summary>
        /// <param name="m"></param>
        void DoHotkey(Message m)
        {
            // Could decode if we needed to handle more than one.

            // Get current window.
            var fgi = GetWindowInfo(WM.ForegroundWindow);
            _logger.Debug($">>> WM_HOTKEY_MESSAGE_ID {fgi}");

            // Show UI to let user pick a clip to paste.
            WindowState = FormWindowState.Normal;

            m.Result = -1; // means handled?
        }

        /// <summary>
        /// Add a new clip for viewing.
        /// </summary>
        /// <param name="clip"></param>
        void AddClip(ClipBase clip)
        {

            selector.AddUserItem("", clip.Thumbnail, clip, 0); //TODO1 tooltip text?


            // ClipDisplay clipd = new(clip)
            // {
            //     Width = UserSettings.Settings.ClipSize.Width,
            //     Height = UserSettings.Settings.ClipSize.Height,
            // };

            // clipd.MouseClick += (sender, e) => { ClipClick(sender as ClipDisplay, true, e.Button); };
            // clipd.MouseDoubleClick += (sender, e) => { ClipClick(sender as ClipDisplay, false, e.Button); };
            
            // _clips.Add(clipd);
            
            // Controls.Add(clipd);
        }





        #region Privates
        /// <summary>
        /// Just for debugging.
        /// </summary>
        /// <param name="s"></param>
        void Tell(string s)
        {
            // TODO1 ?? this.InvokeIfRequired(_ => { _log.Add(s + Environment.NewLine); });
        }

        /// <summary>
        /// Edit the options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(_settings, "User Settings", 450);
            MessageBox.Show("Restart required for changes to take effect");
            _settings.Save();
        }
        #endregion





        /// <summary>
        /// Keeps track of foreground window for paste ops.
        /// </summary>
        void WindowEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var winfo = GetWindowInfo(hwnd);

            if (winfo.IsVisible && winfo.ProcessName != "WinClip" && winfo.ProcessName != "explorer")
            {
                _pasteWin = hwnd;
                _dev.Tell($"Set _pasteWin: {_pasteWin} [{GetWindowInfo(_pasteWin).ProcessName}]");
            }
        }




        /// <summary>
        /// Get pertinent bits of info for a window.
        /// </summary>
        /// <param name="hwnd">If 0, my process</param>
        WindowInfo GetWindowInfo(IntPtr hwnd = 0)
        {
            var appInfo = WM.GetAppWindowInfo(hwnd);
            Process? process = hwnd == 0 ? Process.GetCurrentProcess() : Process.GetProcessById(appInfo.Pid);
            var title = appInfo.Title;
            var procName = process.ProcessName;
            WindowInfo winfo = new(hwnd, appInfo.Pid, procName, title, appInfo.IsVisible);
            return winfo;
        }




        // /// <summary>Do debug stuff.</summary>
        // /// <param name="sender"></param>
        // /// <param name="e"></param>
        // void Debug_Click(object sender, EventArgs e)
        // {
        //     _dev.Show();
        //     for (int i = 0; i < 20; i++)
        //     {
        //         var sdir = MiscUtils.GetSourcePath();
        //         var fn = Path.Combine(sdir, "Test", "ross.txt");
        //         AddClip(new PlainTextClip(new DataObject(File.ReadAllText(fn))));
        //         fn = Path.Combine(sdir, "Test", "ex.rtf");
        //         AddClip(new RtfTextClip(new DataObject(RtfTextClip.TYPE_NAME, (File.ReadAllText(fn)))));
        //         fn = Path.Combine(sdir, "Test", "ex.png");
        //         AddClip(new ImageClip(new DataObject(Bitmap.FromFile(fn))));
        //     }

        //     // foreach (var clipd in _clips)
        //     // {
        //     //     _dev.Tell(clipd.Clip.Format());
        //     // }

        //     //List<IntPtr> appwins = WM.GetTopWindows(false);
        //     //foreach (IntPtr appwin in appwins)
        //     //{
        //     //    var winfo = GetWindowInfo(appwin);
        //     //    tvInfo.Append(winfo.ToString());
        //     //}
        // }


        #region Native
        private const int WINEVENT_INCONTEXT = 4;
        private const int WINEVENT_OUTOFCONTEXT = 0;
        private const int WINEVENT_SKIPOWNPROCESS = 2;
        private const int WINEVENT_SKIPOWNTHREAD = 1;
        private const int EVENT_SYSTEM_FOREGROUND = 3;

        private delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, int dwflags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern uint GetClipboardSequenceNumber();
        #endregion
    }
}
