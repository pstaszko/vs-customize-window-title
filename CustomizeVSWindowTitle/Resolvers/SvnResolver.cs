using System;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class SvnResolver : SimpleTagResolver {

        public const string SvnExecFn = "svn.exe";
        private static string SvnExecFp = SvnExecFn;

        public SvnResolver() : base(tagName: "svnDirectoryName") { }

        public override string Resolve(AvailableInfo info) {
            UpdateSvnExecFp(info.GlobalSettings.SvnDirectory);
            return GetSvnDirectoryOrEmpty(info.Solution);
        }
        public static void UpdateSvnExecFp(string svnDp) {
            if (string.IsNullOrEmpty(svnDp)) {
                SvnExecFp = SvnExecFn;
                return;
            }
            SvnExecFp = Path.Combine(svnDp, SvnExecFn);
        }

        public static string GetSvnDirectoryOrEmpty(Solution solution) {
            var sn = solution?.FullName;
            if (string.IsNullOrEmpty(sn)) return string.Empty;
            var workingDirectory = new FileInfo(sn).DirectoryName;
            return GetSvnDirectory(workingDirectory) ?? string.Empty;
        }

        public static string GetSvnDirectory(string workingDirectory) {
            try {
                using (var pProcess = new System.Diagnostics.Process {
                    StartInfo = {
                        FileName = SvnExecFp,
                        Arguments = "info",
                        UseShellExecute = false,
                        StandardOutputEncoding = Encoding.UTF8,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDirectory
                    }
                }) {
                    pProcess.Start();
                    var output = pProcess.StandardOutput.ReadToEnd();
                    pProcess.WaitForExit();
                    if (pProcess.ExitCode != 0) {
                        // Not a working directory.
                        return null;
                    }
                    const string Prefix = "Relative URL: ";
                    var branchLine = output.Split('\n').SingleOrDefault(line => line.StartsWith(Prefix, StringComparison.Ordinal));
                    return branchLine?.Substring(Prefix.Length).Trim('^', '\n', '\r', ' ');
                }
            }
            catch (Exception ex) {
                try {
                    if (CustomizeVSWindowTitle.CurrentPackage.UiSettings.EnableDebugMode) {
                        CustomizeVSWindowTitle.WriteOutput("SvnResolver.GetSvnDirectory() exception: " + ex);
                    }
                }
                catch {
                    // ignored
                }
                return null;
            }
        }
    }
}