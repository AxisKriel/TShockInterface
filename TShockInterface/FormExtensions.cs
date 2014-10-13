using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TShockAPI;
using TShockAPI.Hooks;

namespace TShockInterface
{
    public static class FormExtensions
    {
        public static void MainThreadInvoke(this Control control, Action func)
        {
            if (control.InvokeRequired)
                control.Invoke(func);
            else func();
        }

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

        public static bool HandleCommand(TSPlayer player, string text)
        {
            string cmdText = text.Remove(0, 1);

            var args = ParseParameters(cmdText);
            if (args.Count < 1)
                return false;

            string cmdName = args[0].ToLower();
            args.RemoveAt(0);

            IEnumerable<Command> cmds = Commands.ChatCommands.Where(c => c.HasAlias(cmdName)).ToList();

            if (TShockAPI.Hooks.PlayerHooks.OnPlayerCommand(player, cmdName, cmdText, args, ref cmds))
                return true;

            if (cmds.Count() == 0)
            {
                if (player.AwaitingResponse.ContainsKey(cmdName))
                {
                    Action<CommandArgs> call = player.AwaitingResponse[cmdName];
                    player.AwaitingResponse.Remove(cmdName);
                    call(new CommandArgs(cmdText, player, args));
                    return true;
                }
                player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", TShock.Config.CommandSpecifier);
                return true;
            }
            foreach (Command cmd in cmds)
            {
                if (!cmd.CanRun(player))
                {
                    TShock.Utils.SendLogs(string.Format("{0} tried to execute {1}{2}.", player.Name, TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                    player.SendErrorMessage("You do not have access to this command.");
                }
                else if (!cmd.AllowServer && !player.RealPlayer)
                {
                    player.SendErrorMessage("You must use this command in-game.");
                }
                else
                {
                    if (cmd.DoLog)
                        TShock.Utils.SendLogs(string.Format("{0} executed: {1}{2}.", player.Name, TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                    cmd.Run(cmdText, player, args);
                }
            }
            return true;
        }

        private static List<String> ParseParameters(string str)
        {
            var ret = new List<string>();
            var sb = new StringBuilder();
            bool instr = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == '\\' && ++i < str.Length)
                {
                    if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
                        sb.Append('\\');
                    sb.Append(str[i]);
                }
                else if (c == '"')
                {
                    instr = !instr;
                    if (!instr)
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (sb.Length > 0)
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else if (IsWhiteSpace(c) && !instr)
                {
                    if (sb.Length > 0)
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else
                    sb.Append(c);
            }
            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret;
        }

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }
    }
}
