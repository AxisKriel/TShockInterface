using System;
using System.Runtime.InteropServices;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using System.Threading;

namespace TShockInterface
{  
    [ApiVersion(1,16)]
    public class TSInterface : TerrariaPlugin
    {
        public static bool FormOpen;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public  const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public static int ConsoleState = 0;

        public override string Author { get { return "Ancientgods"; } }

        public override string Name { get { return "Interface"; } }

        public override string Description { get { return ""; } }

        public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(OpenInterface, "open"));
        }

        public TSInterface(Main game) : base(game)
        {
            Order = 1;
        }

        public static void ToggleConsoleState()
        {
            var handle = GetConsoleWindow();
            ConsoleState = ConsoleState == 5 ? 0 : 5;
            ShowWindow(handle, ConsoleState);
        }

        public void SetConsoleState(int i)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, i);
        }

        public void OpenInterface(CommandArgs args)
        {
            if (!(args.Player  is TShockAPI.TSServerPlayer))
            {
                args.Player.SendErrorMessage("Only the console can do that!");
                return;
            }
            if (FormOpen)
            {
                args.Player.SendErrorMessage("Window is already open!");
                return;
            }
            Thread t = new Thread(() =>
            {
                Window w = new Window();
                try
                {
                    FormOpen = true;
                    ServerApi.Hooks.ServerChat.Register(this, w.OnChat);
                    ServerApi.Hooks.NetGreetPlayer.Register(this, w.OnGreetPlayer);
                    ServerApi.Hooks.ServerLeave.Register(this, w.OnLeave);
                    SetConsoleState(SW_HIDE);
                    w.ShowDialog();
                }
                catch (Exception ex)
                {
                    Log.ConsoleError("window closed because it crashed: " + ex.ToString());
                }
                ServerApi.Hooks.ServerChat.Deregister(this, w.OnChat);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, w.OnGreetPlayer);
                ServerApi.Hooks.ServerLeave.Deregister(this, w.OnLeave);
                SetConsoleState(SW_SHOW);
                FormOpen = false;
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}
