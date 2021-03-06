//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

// Created by Petr Bena benapetr@gmail.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace wmib
{
    /// <summary>
    /// Configuration
    /// </summary>
    public partial class config
    {
        /// <summary>
        /// This is a temporary string containing the configuration data
        /// </summary>
        private static string text = null;

        /// <summary>
        /// Network the bot is connecting to
        /// </summary>
        public static string network = "irc.freenode.net";

        /// <summary>
        /// Nick name
        /// </summary>
        public static string username = "wm-bot";

        /// <summary>
        /// Uptime
        /// </summary>
        public static System.DateTime UpTime;

        /// <summary>
        /// Debug channel (doesn't need to exist)
        /// </summary>
        public static string debugchan = null;

        /// <summary>
        /// Link to css
        /// </summary>
        public static string css;

        /// <summary>
        /// Login name
        /// </summary>
        public static string login = "";

        /// <summary>
        /// Login pw
        /// </summary>
        public static string password = "";

        /// <summary>
        /// Whether the bot is using external network module
        /// </summary>
        public static bool UsingNetworkIOLayer = false;

        /// <summary>
        /// The webpages url
        /// </summary>
        public static string WebpageURL = "";

        /// <summary>
        /// Dump
        /// </summary>
        public static string DumpDir = "dump";

        /// <summary>
        /// This is a log where network log is dumped to
        /// </summary>
        public static string TransactionLog = "transaction.dat";

        /// <summary>
        /// Path to txt logs
        /// </summary>
        public static string path_txt = "log";

        /// <summary>
        /// Network traffic is logged
        /// </summary>
        public static bool Logging = false;

        /// <summary>
        /// Path to html which is generated by this process
        /// </summary>
        public static string path_htm = "html";

        /// <summary>
        /// Version
        /// </summary>
        public static string version = "wikimedia bot v. 1.10.8.12";

        /// <summary>
        /// Separator for system db
        /// </summary>
        public static string separator = "|";

        /// <summary>
        /// User name
        /// </summary>
        public static string name = "wm-bot";

        /// <summary>
        /// List of channels the bot is in
        /// </summary>
        public static List<channel> channels = new List<channel>();

        /// <summary>
        /// This is a string which commands are prefixed with
        /// </summary>
        public const string CommandPrefix = "@";

        /// <summary>
        /// Add line to the config file
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private static void AddConfig(string a, string b)
        {
            text = text + "\n" + a + "=" + b + ";";
        }

        /// <summary>
        /// How verbose the debugging is
        /// </summary>
        public static int SelectedVerbosity = 0;

        /// <summary>
        /// Save a channel config
        /// </summary>
        public static void Save()
        {
            text = "";
            AddConfig("username", username);
            AddConfig("password", password);
            AddConfig("web", WebpageURL);
            AddConfig("serverIO", UsingNetworkIOLayer.ToString());
            AddConfig("debug", debugchan);
            AddConfig("network", network);
            AddConfig("style_html_file", css);
            AddConfig("nick", login);
            text += "\nchannels=";
            lock (config.channels)
            {
                foreach (channel current in channels)
                {
                    text += current.Name + ",\n";
                }
            }
            text = text + ";";
            File.WriteAllText(variables.config + "/wmib", text);
            text = null;
        }

        /// <summary>
        /// Parse config data text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string parseConfig(string text, string name)
        {
            if (text.Contains(name + "="))
            {
                string x = text;
                x = text.Substring(text.IndexOf(name + "=")).Replace(name + "=", "");
                if (x.Contains(";"))
                {
                    x = x.Substring(0, x.IndexOf(";"));
                    return x;
                }
            }
            return "";
        }

        /// <summary>
        /// Return a temporary name
        /// </summary>
        /// <param name="file">File you need to have temp for</param>
        /// <returns></returns>
        public static string tempName(string file)
        {
            return (file + "~");
        }

        /// <summary>
        /// Load config of bot
        /// </summary>
        public static int Load()
        {
            try
            {
                if (Directory.Exists(variables.config) == false)
                {
                    Directory.CreateDirectory(variables.config);
                }
                if (!System.IO.File.Exists(variables.config + "/wmib"))
                {
                    System.IO.File.WriteAllText(variables.config + "/wmib", "//this is configuration file for bot, you need to fill in some stuff for it to work");
                }
                text = File.ReadAllText(variables.config + "/wmib");
                bool _serverIO;
                if (bool.TryParse(parseConfig(text, "serverIO"), out _serverIO))
                {
                    UsingNetworkIOLayer = _serverIO;
                }
                foreach (string x in parseConfig(text, "channels").Replace("\n", "").Split(','))
                {
                    string name = x.Replace(" ", "").Replace("\n", "");
                    if (name != "")
                    {
                        lock (channels)
                        {
                            channels.Add(new channel(name));
                        }
                    }
                }
                Program.Log("Channels were all loaded");

                // Now when all chans are loaded let's link them together
                foreach (channel ch in channels)
                {
                    ch.Shares();
                }
                Program.Log("Channel db's working");
                username = parseConfig(text, "username");
                network = parseConfig(text, "network");
                login = parseConfig(text, "nick");
                debugchan = parseConfig(text, "debug");
                css = parseConfig(text, "style_html_file");
                WebpageURL = parseConfig(text, "web");
                password = parseConfig(text, "password");
                if (login == "")
                {
                    Console.WriteLine("Error there is no username for bot");
                    return 1;
                }
                if (network == "")
                {
                    Console.WriteLine("Error irc server is wrong");
                    return 1;
                }
                if (username == "")
                {
                    Console.WriteLine("Error there is no username for bot");
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                core.handleException(ex);
            }
            if (!Directory.Exists(DumpDir))
            {
                Directory.CreateDirectory(DumpDir);
            }
            return 0;
        }
    }
}
