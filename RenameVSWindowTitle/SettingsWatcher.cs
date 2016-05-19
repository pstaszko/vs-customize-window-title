using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    /// <summary>
    /// This class maintains a list of config sets loaded from a single config file.
    /// File changes are observed by a filesystem watcher and reloaded on next demand.
    /// 
    /// </summary>
    public class SettingsWatcher
    {
        readonly bool IsGlobalConfig;

        //loaded sets (null means reload pending, empty list means missing or error)
        List<SettingsSet> Sets;
        bool NeedReload = false;
        string SettingsPath;
        FileSystemWatcher Watcher;

        static readonly char[] PathSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        private bool v;
        public delegate void SettingsClearedDelegate();
        public SettingsClearedDelegate SettingsCleared;

        public SettingsWatcher(bool fIsGlobalConfig)
        {
            this.IsGlobalConfig=fIsGlobalConfig;
        }

        public void Clear()
        {
            Sets=null;
            NeedReload=true;
            stopWatching();
            if (SettingsCleared!=null)
                SettingsCleared();
        }

        private void stopWatching()
        {
            if (Watcher!=null)
            {
                // stop watching
                Watcher.EnableRaisingEvents=false;
                Watcher.Dispose();
                Watcher=null;
            }
        }

        public void Update(string path)
        {
            if (SettingsPath==path)
                return;
            Clear();
            SettingsPath=path;
        }

        public bool TryUpdate(SettingsSet cfg)
        {
            if (NeedReload)
            {
                reloadConfig();
            }

            foreach (SettingsSet S in Sets)
            {
                bool bMatched;
                if (!IsGlobalConfig)
                {
                    // ignore paths
                    bMatched=true;
                }
                else
                {
                    bMatched=false;
                    foreach (string P in S.Paths)
                    {
                        bMatched =
                        (-1==P.IndexOfAny(PathSeparators)) ?
                         P.Equals(cfg.SolutionFileName, StringComparison.CurrentCultureIgnoreCase) :
                         P.Equals(cfg.SolutionFilePath, StringComparison.CurrentCultureIgnoreCase);
                        if (bMatched)
                            break;
                    }
                }
                if (bMatched)
                {
                    cfg.Merge(S);
                    return true;
                }
            }
            return false;
        }

        private void reloadConfig()
        {
            NeedReload=false;
            if (string.IsNullOrEmpty(SettingsPath))
                return;

            stopWatching();
            Watcher =new FileSystemWatcher(Path.GetDirectoryName(SettingsPath), Path.GetFileName(SettingsPath));
            Watcher.Changed+=Watcher_Changed;
            Watcher.Renamed+=Watcher_Renamed;
            Watcher.Deleted+=Watcher_Deleted;

            loadConfig();

            Watcher.EnableRaisingEvents=true;
        }

        private static string GetAttributeOrChild(XmlElement node, string name)
        {
            var attr = node.GetAttributeNode(name);
            if (attr!=null)
                return attr.Value;
            var child = node.GetElementsByTagName(name);
            if (child!=null && child.Count>0)
                return child[0].InnerText;
            return null;
        }

        private static void cfgUpdate<T>(ref T target, XmlElement node, string name)
        {
            string val = GetAttributeOrChild(node, name);
            if (string.IsNullOrEmpty(val))
                return;
            target=(T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }


        private void loadConfig()
        {
            Sets=new List<SettingsSet>();

            if (!File.Exists(SettingsPath))
            {
                Debug.WriteLine("No settings overrides {0}", SettingsPath, null);
                return;
            }

            XmlDocument doc = new XmlDocument();
            Debug.WriteLine("Read settings overrides from {0}", SettingsPath, null);
            try
            {
                doc.Load(SettingsPath);
            }
            catch (Exception x)
            {
                Debug.WriteLine("Error {0}", x, null);

            }

            var nodes = doc.SelectNodes("RenameVSWindowTitle/SettingsSet");

            foreach (XmlElement node in nodes)
            {
                SettingsSet cfg = new SettingsSet();

                // read paths (Path attribute and Path child elements)
                cfg.Paths=new List<string>();
                string path = node.GetAttribute(Globals.TagPath);
                if (!string.IsNullOrEmpty(path))
                    cfg.Paths.Add(path);

                var paths = node.GetElementsByTagName(Globals.TagPath);
                if (paths!=null && paths.Count>0)
                {
                    foreach (XmlElement elem in paths)
                    {
                        path=elem.InnerText;
                        if (!string.IsNullOrEmpty(path))
                            cfg.Paths.Add(path);
                    }
                }

                cfgUpdate(ref cfg.SolutionName, node, Globals.TagSolutionName);
                cfgUpdate(ref cfg.ClosestParentDepth, node, Globals.TagClosestParentDepth);
                cfgUpdate(ref cfg.FarthestParentDepth, node, Globals.TagFarthestParentDepth);
                cfgUpdate(ref cfg.AppendedString, node, Globals.TagAppendedString);
                cfgUpdate(ref cfg.PatternIfRunningMode, node, Globals.TagPatternIfRunningMode);
                cfgUpdate(ref cfg.PatternIfBreakMode, node, Globals.TagPatternIfBreakMode);
                cfgUpdate(ref cfg.PatternIfDesignMode, node, Globals.TagPatternIfDesignMode);

                Sets.Add(cfg);
            }
        }
   
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Clear();
            NeedReload=false;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Clear();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Clear();
        }
    }
}
