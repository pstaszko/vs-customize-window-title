using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ErwinMayerLabs.Lib;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class PathResolver : TagResolver, ISimpleTagResolver {
        private const string tagName = "path";

        public PathResolver() : base(tagNames: new[] { tagName, tagName + ":X", tagName + ":X:Y" }) { }

        public string TagName { get; } = tagName;

        public string Resolve(AvailableInfo info) {
            return string.IsNullOrEmpty(info.Path) ? info.WindowName : info.Path;
        }

        public override bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            if (!tag.StartsWith(tagName, StringComparison.InvariantCulture)) return false;
            var m = Globals.RangeRegex.Match(tag.Substring(tagName.Length));
            if (m.Success) {
                if (!info.PathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var startIndex = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["startIndex"].Value, CultureInfo.InvariantCulture)));
                    var endIndex = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["endIndex"].Value, CultureInfo.InvariantCulture)));
                    var pathRange = info.PathParts.GetRange(startIndex: startIndex, endIndex: endIndex).ToArray();
                    s = Globals.GetPathForTitle(pathRange);
                }
                return true;
            }
            m = Globals.IndexRegex.Match(tag.Substring(tagName.Length));
            if (m.Success) {
                if (!info.PathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var index = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["index"].Value, CultureInfo.InvariantCulture)));
                    s = info.PathParts[index];
                }
                return true;
            }
            return false;
        }
    }

    public class ParentResolver : TagResolver {
        private const string tagName = "parent";

        public ParentResolver() : base(tagNames: new[] { tagName + ":X", tagName + ":X:Y" }) { }

        public override bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            if (!tag.StartsWith(tagName, StringComparison.InvariantCulture)) return false;

            var m = Globals.RangeRegex.Match(tag.Substring(tagName.Length));
            if (m.Success) {
                if (!info.PathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var startIndex = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["startIndex"].Value, CultureInfo.InvariantCulture)));
                    var endIndex = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["endIndex"].Value, CultureInfo.InvariantCulture)));
                    var pathRange = info.PathParts.GetRange(startIndex: info.PathParts.Length - 1 - startIndex, endIndex: info.PathParts.Length - 1 - endIndex).ToArray();
                    s = Globals.GetPathForTitle(pathRange);
                }
                return true;
            }
            m = Globals.IndexRegex.Match(tag.Substring(tagName.Length));
            if (m.Success) {
                if (!info.PathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var index = Math.Min(info.PathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["index"].Value, CultureInfo.InvariantCulture)));
                    s = info.PathParts[info.PathParts.Length - 1 - index];
                }
                return true;
            }
            return false;
        }
    }

    public class ParentPathResolver : SimpleTagResolver {
        public ParentPathResolver() : base(tagName: "parentPath") { }

        public override string Resolve(AvailableInfo info) {
            return AvailableInfo.GetParentPath(info.PathParts, info.Cfg?.ClosestParentDepth ?? info.GlobalSettings.ClosestParentDepth, info.Cfg?.FarthestParentDepth ?? info.GlobalSettings.FarthestParentDepth) ?? string.Empty;
        }
    }

    public class EnvResolver : TagResolver {
        static readonly Regex EnvRegex = new Regex(@"^env:(.+)$", RegexOptions.Compiled);

        public EnvResolver() : base(tagNames: new[] { "env:X" }) { }

        public override bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            var m = EnvRegex.Match(tag);
            if (m.Success) {
                s = Environment.GetEnvironmentVariable(m.Groups[1].Value);
                return true;
            }
            return false;
        }
    }
}