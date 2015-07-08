/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <lyserbin@LucianSoft.ml> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return lyserbin
 * ----------------------------------------------------------------------------
 */

using MinimalisticTelnet;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace NowPlayingVLC
{
    class Program
    {
        private static ContextMenu trayMenu;
        private static NotifyIcon trayIcon;
        private static Thread worker;
        private static Settings settings;
        private static TelnetConnection tc;

        static void Main(string[] args)
        {
            try
            {
                settings = Settings.Load("nowplayingvlc.conf.xml");
            }
            catch (Exception)
            {
                settings = new Settings();
            }
            finally
            {
                try
                {
                    Settings.Save(settings, "nowplayingvlc.conf.xml");
                } catch { } // WHAT THE HELL IS WRONG WITH YOU
            }

            string tmp;

            try
            {
                tc = new TelnetConnection(settings.Hostname, (int)settings.Port);
                tmp = tc.Login(settings.Password, 100);
                if (tmp[tmp.Length - 2] == ':')
                {
                    MessageBox.Show("Invalid password!", "NowPlayingVLC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                worker = new Thread(work);
                worker.Start();

                // Tray Icon stuff
                trayMenu = new ContextMenu();
                trayMenu.MenuItems.Add("Exit", new EventHandler(exit));
                trayIcon = new NotifyIcon();
                trayIcon.Text = "NowPlayingVLC";
                trayIcon.Icon = new Icon(NowPlayingVLC.Properties.Resources.nigga_christ, 40, 40);
                trayIcon.Visible = true;
                trayIcon.ContextMenu = trayMenu;
                Application.Run(); // the application freeze here.
            }
            catch (SocketException)
            {
                MessageBox.Show("Can't connect to the VLC server!", "NowPlayingVLC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void work(object obj)
        {
            string old = "";
            while(true)
            {
                string currentsong;

                tc.WriteLine("get_title");
                currentsong = tc.Read();
                if (currentsong != old)
                {
                    old = currentsong; // cache BRO

                    currentsong = currentsong.Substring(0, currentsong.Length - 4);
                    if (currentsong.Length > 4 && (currentsong[currentsong.Length - 4] == '.'))
                        currentsong = currentsong.Substring(0, currentsong.Length - 4);

                    File.WriteAllText(settings.FilePath, currentsong);
                }

                Thread.Sleep((int)settings.Delay);
            }
        }

        private static void exit(object sender, EventArgs e)
        {
            worker.Abort();
            tc.WriteLine("quit");
            tc.Read();
            trayIcon.Dispose();
            Application.Exit();
        }
    }
}
