using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using System;
using System.Reflection;
using System.IO;

namespace TERAResources
{
    public class TeraItem
    {
        public static readonly List<TeraItem> ItemList = new List<TeraItem>();
        private static readonly List<string> Icons = new List<string>();

        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Image ImageIcon { get; private set; }

        public static void LoadXMLItems(bool loadIcons)
        {
            foreach (string name in typeof(TeraItem).Assembly.GetManifestResourceNames())
            {
                if (!name.Contains("TERAResources.Resources."))
                    continue;
                Icons.Add(name);
            }

            Assembly assembly = typeof(TeraItem).Assembly;
            Stream stream = typeof(TeraItem).Assembly.GetManifestResourceStream("TERAResources.Items.xml");
            XDocument document = XDocument.Load(stream);
            ItemList.AddRange(from item in document.Root.Descendants("item")
                              select new TeraItem()
                              {
                                  Id = int.Parse(item.Attribute("id").Value),
                                  Name = item.Attribute("name").Value,
                                  Icon = item.Attribute("icon").Value,
                                  ImageIcon = GetImageIcon(item.Attribute("icon").Value, assembly, loadIcons)
                              });
        }

        private static Image GetImageIcon(string icon, Assembly assembly, bool loadIcons)
        {
            if(loadIcons && Icons.Exists(i => i == $"TERAResources.Resources.{icon.ToLower()}.png"))
            {
                return new Bitmap(assembly.GetManifestResourceStream($"TERAResources.Resources.{icon.ToLower()}.png"));
            }
            return new Bitmap(assembly.GetManifestResourceStream("TERAResources.Resources.unknown.png"));
        }
    }
}
