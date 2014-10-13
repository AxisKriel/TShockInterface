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
using TerrariaApi.Server;

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
                else
                {
                    TShock.Utils.Broadcast("(Server Broadcast) " + msg, Convert.ToByte(TShock.Config.BroadcastRGB[0]), Convert.ToByte(TShock.Config.BroadcastRGB[1]), Convert.ToByte(TShock.Config.BroadcastRGB[2]));
                    richTextBox1.Append("(Server Broadcast) " + msg, Convert.ToByte(TShock.Config.BroadcastRGB[0]), Convert.ToByte(TShock.Config.BroadcastRGB[1]), Convert.ToByte(TShock.Config.BroadcastRGB[2]));
                }
            }
        }

        public void OnChat(ServerChatEventArgs e)
        {
            TSPlayer tsplr = TShock.Players[e.Who];
            if (tsplr.mute)
                return;

            var text = String.Format(TShock.Config.ChatFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix, e.Text);
            richTextBox1.Append(text, tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
        }

        public void OnGreetPlayer(GreetPlayerEventArgs e)
        {
            TSPlayer tsplr= TShock.Players[e.Who];
            richTextBox1.Append(string.Format("{0} has joined. IP: {1}", tsplr.Name,tsplr.IP), System.Drawing.Color.Yellow);
        }

        public void OnLeave(LeaveEventArgs e)
        {
            TSPlayer tsplr = TShock.Players[e.Who];
            richTextBox1.Append(tsplr.Name + " has left.", System.Drawing.Color.Yellow);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSInterface.ToggleConsoleState();
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
        }
    }
}
