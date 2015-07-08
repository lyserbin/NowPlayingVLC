/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <lyserbin@LucianSoft.ml> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return lyserbin
 * ----------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.Xml.Serialization;

namespace NowPlayingVLC
{
    public class Settings
    {
        public string Hostname { get; set; }

        public uint Port { get; set; }

        public string Password { get; set; }

        public string FilePath { get; set; }

        public uint Delay { get; set; }

        public Settings()
        {
            this.Hostname = "localhost";
            this.Port = 4212;
            this.Password = "password";
            this.FilePath = "nowplayingvlc.txt";
            this.Delay = 3000;
        }
        public static Settings Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            FileStream stream = new FileStream(filename, FileMode.Open);
            return (Settings)serializer.Deserialize(stream);
        }

        public static void Save(Settings settings, string filename)
        {
            XmlSerializer serializer =new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, settings);
            writer.Close();
        }
    }
}
