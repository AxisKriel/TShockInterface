using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TerrariaApi;
using TShockAPI;
using TShockAPI.Hooks;
using TerrariaApi.Server;
using System.Threading;
using System.IO.Streams;
using System.IO;

namespace TShockInterface
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TextBox tb = (sender as TextBox);
                string msg = tb.Text;
                tb.Clear();
                if (msg.StartsWith("/") || msg.StartsWith("say") || msg.StartsWith("who") || msg.StartsWith("online") || msg.StartsWith("playing"))
                {
                    Commands.HandleCommand(TSPlayer.Server, msg);
                }
                else if(msg.Equals("cls") || msg.Equals("clear"))
                    richTextBox1.Clear();
                else
                    TShock.Utils.Broadcast("(Server Broadcast) " + msg, Convert.ToByte(TShock.Config.BroadcastRGB[0]), Convert.ToByte(TShock.Config.BroadcastRGB[1]), Convert.ToByte(TShock.Config.BroadcastRGB[2]));
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void confirmToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TShock.Utils.StopServer(false);
            this.Close();
        }

        private void confirmToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TShock.Utils.StopServer(true);
            this.Close();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            textBox1.ForeColor = System.Drawing.Color.FromArgb(Convert.ToByte(TShock.Config.BroadcastRGB[0]), Convert.ToByte(TShock.Config.BroadcastRGB[1]), Convert.ToByte(TShock.Config.BroadcastRGB[2]));

            Action<string> WriteToTextbox = (s) =>
                {
                    if (s.Length > 0)
                    {
                        if ((int)s[0] == 84)
                            richTextBox1.Clear();
                        else
                            richTextBox1.Append(s);
                    }
                };
            Console.SetOut(new InterceptingWriter(Console.Out, WriteToTextbox));
        }
    }
}
