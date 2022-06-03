using System;
using System.Xml;

namespace TERACalendar
{
    internal class Configuration
    {
        public static string DbUrl { get; private set; }
        public static string DbUser { get; private set; }
        public static string DbPassword { get; private set; }
        public static bool LoadIcons { get; private set; }

        public static void LoadConfiguration()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"Configuration.xml");
            XmlNode configurationNode = xmlDocument.SelectSingleNode("/Configuration");

            foreach (XmlNode node in configurationNode)
            {
                switch (node.Name)
                {
                    case "Sql":
                        DbUrl = node.Attributes["url"].Value;
                        DbUser = node.Attributes["user"].Value;
                        DbPassword = node.Attributes["password"].Value;
                        break;
                    case "App":
                        LoadIcons = bool.Parse(node.Attributes["loadIcons"].Value);
                        break;
                }
            }
        }
    }
}
