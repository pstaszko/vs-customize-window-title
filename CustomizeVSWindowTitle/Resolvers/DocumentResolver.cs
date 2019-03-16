using System;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using ErwinMayerLabs.Lib;

namespace ErwinMayerLabs.CustomizeVSWindowTitleExtension.Resolvers {
    public static class DocumentHelper {
        public static string GetActiveDocumentProjectNameOrEmpty(Document activeDocument) {
            return activeDocument?.ProjectItem?.ContainingProject?.Name ?? string.Empty;
        }

        public static string GetActiveDocumentProjectFileNameOrEmpty(Document activeDocument) {
            var fn = activeDocument?.ProjectItem?.ContainingProject?.FullName;
            return fn != null ? Path.GetFileName(fn) : string.Empty;
        }

        public static string GetActiveDocumentNameOrEmpty(Document activeDocument) {
            return activeDocument != null ? Path.GetFileName(activeDocument.FullName) : string.Empty;
        }

        public static string GetActiveWindowNameOrEmpty(Window activeWindow) {
            if (activeWindow != null && activeWindow.Caption != Globals.DTE.MainWindow.Caption) {
                return activeWindow.Caption ?? string.Empty;
            }
            return string.Empty;
        }

        public static string GetActiveDocumentPathOrEmpty(Document activeDocument) {
            return activeDocument != null ? activeDocument.FullName : string.Empty;
        }
    }

    public class DocumentNameResolver : SimpleTagResolver {
        public DocumentNameResolver() : base(tagName: "documentName") { }

        public override string Resolve(AvailableInfo info) {
            return string.IsNullOrEmpty(info.DocumentName) ? info.WindowName : info.DocumentName;
        }
    }

    public class DocumentPathResolver : TagResolver, ISimpleTagResolver {
        public string TagName { get; } = "documentPath";

        public DocumentPathResolver() : base(tagNames: new[] { "documentPath", "documentPath:X", "documentPath:X:Y" }) { }

        public string Resolve(AvailableInfo info) {
            return string.IsNullOrEmpty(info.DocumentName) ? info.WindowName : info.DocumentPath;
        }

        public override bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            if (!tag.StartsWith("documentPath", StringComparison.InvariantCulture)) return false;

            var m = Globals.RangeRegex.Match(tag.Substring("documentPath".Length));
            if (m.Success) {
                if (!info.DocumentPathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var startIndex = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["startIndex"].Value, CultureInfo.InvariantCulture)));
                    var endIndex = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["endIndex"].Value, CultureInfo.InvariantCulture)));
                    var pathRange = info.DocumentPathParts.GetRange(startIndex: startIndex, endIndex: endIndex).ToArray();
                    s = Globals.GetPathForTitle(pathRange);
                }
                return true;
            }
            m = Globals.IndexRegex.Match(tag.Substring("documentPath".Length));
            if (m.Success) {
                if (!info.DocumentPathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var index = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["index"].Value, CultureInfo.InvariantCulture)));
                    s = info.DocumentPathParts[index];
                }
                return true;
            }
            return false;
        }
    }

    public class DocumentParentPathResolver : TagResolver {
        public DocumentParentPathResolver() : base(tagNames: new[] { "documentParentPath:X", "documentParentPath:X:Y" }) { }

        public override bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            if (!tag.StartsWith("documentParentPath", StringComparison.InvariantCulture)) return false;
            
            var m = Globals.RangeRegex.Match(tag.Substring("documentParentPath".Length));
            if (m.Success) {
                if (!info.DocumentPathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var startIndex = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["startIndex"].Value, CultureInfo.InvariantCulture)));
                    var endIndex = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["endIndex"].Value, CultureInfo.InvariantCulture)));
                    var pathRange = info.DocumentPathParts.GetRange(startIndex: info.DocumentPathParts.Length - 1 - startIndex, endIndex: info.DocumentPathParts.Length - 1 - endIndex).ToArray();
                    s = Globals.GetPathForTitle(pathRange);
                }
                return true;
            }
            m = Globals.IndexRegex.Match(tag.Substring("documentParentPath".Length));
            if (m.Success) {
                if (!info.DocumentPathParts.Any()) {
                    s = string.Empty;
                }
                else {
                    var index = Math.Min(info.DocumentPathParts.Length - 1, Math.Max(0, int.Parse(m.Groups["index"].Value, CultureInfo.InvariantCulture)));
                    s = info.DocumentPathParts[info.DocumentPathParts.Length - 1 - index];
                }
                return true;
            }
            return false;
        }
    }

    public class DocumentProjectFileNameResolver : SimpleTagResolver {
        public DocumentProjectFileNameResolver() : base(tagName: "documentProjectFileName") { }

        public override string Resolve(AvailableInfo info) {
            return DocumentHelper.GetActiveDocumentProjectFileNameOrEmpty(activeDocument: info.ActiveDocument);
        }
    }

    public class DocumentProjectNameResolver : SimpleTagResolver {
        public DocumentProjectNameResolver() : base(tagName: "documentProjectName") { }

        public override string Resolve(AvailableInfo info) {
            return DocumentHelper.GetActiveDocumentProjectNameOrEmpty(activeDocument: info.ActiveDocument);
        }
    }
}