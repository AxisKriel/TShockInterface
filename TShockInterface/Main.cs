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
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public  const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public override string Author { get { return "Ancientgods"; } }

        public override string Name { get { return "Interface"; } }

        public override string Description { get { return ""; } }

        public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public override void Initialize()
        {
            LaunchInterface();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        public TSInterface(Main game) : base(game)
        {
            Order = -1;
        }

        public void SetConsoleState(int i)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, i);
        }

        public void LaunchInterface()
        {
            Thread t = new Thread(() =>
            {
                Window w = new Window();
                try
                {
                    SetConsoleState(SW_HIDE);
                    w.ShowDialog();
                }
                catch (Exception ex)
                {
                    Log.ConsoleError("window closed because it crashed: " + ex.ToString());
                }
                Environment.Exit(0);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}
