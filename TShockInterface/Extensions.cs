using System;
using System.Text;
using System.Windows.Forms;
using TShockAPI;
using System.IO;

namespace TShockInterface
{
    public static class Extensions
    {
        public static void Append(this RichTextBox rtb, string text)
        {
            Append(rtb, text, Convert.ToByte(TShock.Config.BroadcastRGB[0]), Convert.ToByte(TShock.Config.BroadcastRGB[1]), Convert.ToByte(TShock.Config.BroadcastRGB[2]));
        }

        public static void Append(this RichTextBox rtb, string text, int r, int g, int b)
        {
            Append(rtb, text, System.Drawing.Color.FromArgb(r, g, b));
        }

        public static void Append(this RichTextBox rtb, string text, System.Drawing.Color c)
        {
            rtb.SelectionColor = c;
            rtb.AppendText(text + "\r\n");
            rtb.ScrollToCaret();
        }      
    }

    class InterceptingWriter : TextWriter
    {
        TextWriter _existingWriter;
        Action<string> _writeTask;

        public InterceptingWriter(TextWriter existing, Action<string> task)
        {
            _existingWriter = existing;
            _writeTask = task;
        }

        public override void WriteLine(string value)
        {
            _existingWriter.WriteLine(value);
            _writeTask(value);
        }

        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}
