using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;

namespace ErwinMayerLabs.CustomizeVSWindowTitleExtension.Resolvers {
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

    public class StartupProjectNamesResolver : TagResolver, ISimpleTagResolver {
        public StartupProjectNamesResolver() : base(tagNames: new[] { "startupProjectsNames", "startupProjectsNames:X" }) { }

        public string TagName { get; } = "startupProjectsNames";
        static readonly Regex StartupProjectsXRegex = new Regex(@"^startupProjectsNames:(.+)$", RegexOptions.Compiled);

        public string Resolve(AvailableInfo info) {
            return string.Join(" & ", GetStartupProjectNames());
        }

        public override bool TryResolve(string tag, AvailableInfo availableInfo, out string s) {
            var m = StartupProjectsXRegex.Match(tag);
            if (!m.Success) {
                s = null;
                return false;
            }

            s = string.Join(m.Groups[1].Value, GetStartupProjectNames());
            return true;
        }

        private static List<string> GetStartupProjectNames() {
            var sb = (SolutionBuild2)Globals.DTE.Solution.SolutionBuild;
            var names = new List<string>(((Array)sb.StartupProjects).Length);
            foreach (string item in (Array)sb.StartupProjects) {
                //names.Add(Globals.DTE.Solution.Item(item).Name); // Does not work if project is nested in solution folder as only the solution folder is part of the Solution.Projects collection. 
                //If project name is not always equal to project filename, replace with solution here: https://stackoverflow.com/questions/38740773/how-to-get-project-inside-of-solution-folder-in-vsix-project
                names.Add(item.Substring(0, item.Length - Path.GetExtension(item).Length));
            }
            return names;
        }
    }
}
