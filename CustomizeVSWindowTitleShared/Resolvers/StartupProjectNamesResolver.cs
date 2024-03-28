using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using EnvDTE80;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class StartupProjectNamesResolver : TagResolver, ISimpleTagResolver {
        public StartupProjectNamesResolver() : this(tagNames: new[] { "startupProjectsNames", "startupProjectsNames:X" }) { }
        public StartupProjectNamesResolver(IEnumerable<string> tagNames) : base(tagNames: tagNames) { }

        public virtual string TagName { get; } = "startupProjectsNames";
        static readonly Regex StartupProjectsXRegex = new Regex(@"^startupProjectsNames:(.+)$", RegexOptions.Compiled);

        public virtual string Resolve(AvailableInfo info) {
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

        protected static List<string> GetStartupProjectNames() {
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

    public class StartupProjectNamesNonRelativeResolver : StartupProjectNamesResolver {
        public StartupProjectNamesNonRelativeResolver() : base(tagNames: new[] { "startupProjectsNamesNonRelative", "startupProjectsNamesNonRelative:X" }) { }

        public override string TagName { get; } = "startupProjectsNamesNonRelative";
        static readonly Regex StartupProjectsXRegex = new Regex(@"^startupProjectsNamesNonRelative:(.+)$", RegexOptions.Compiled);

        public override string Resolve(AvailableInfo info) {
            return string.Join(" & ", GetStartupProjectNamesNonRelative());
        }

        public override bool TryResolve(string tag, AvailableInfo availableInfo, out string s) {
            var m = StartupProjectsXRegex.Match(tag);
            if (!m.Success) {
                s = null;
                return false;
            }

            s = string.Join(m.Groups[1].Value, GetStartupProjectNamesNonRelative());
            return true;
        }

        private static List<string> GetStartupProjectNamesNonRelative() {
            var names = GetStartupProjectNames();

            for (var i = 0; i < names.Count; i++) {
                string modifiedName = names[i];
                // Strip out folder path so we end up with just the project name
                int index = modifiedName.LastIndexOf("\\");
                if (index > 0) {
                    modifiedName = modifiedName.Substring(index + 1);
                }

                names[i] = modifiedName;
            }

            return names;
        }
    }
}