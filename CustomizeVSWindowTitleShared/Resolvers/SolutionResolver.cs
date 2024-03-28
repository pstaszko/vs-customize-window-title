using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using EnvDTE;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class SolutionNameResolver : SimpleTagResolver {
        public SolutionNameResolver() : base(tagName: "solutionName") { }

        public override string Resolve(AvailableInfo info) {
            return info.Cfg.SolutionName ?? string.Empty;
        }

        public static string GetSolutionNameOrEmpty(Solution solution) {
            var sn = solution?.FullName;
            return string.IsNullOrEmpty(sn) ? "" : Path.GetFileNameWithoutExtension(sn);
        }
    }
}