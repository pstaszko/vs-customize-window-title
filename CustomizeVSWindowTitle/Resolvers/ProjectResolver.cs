using System;
using EnvDTE;
using EnvDTE80;

namespace ErwinMayerLabs.CustomizeVSWindowTitleExtension.Resolvers {
    public static class ProjectHelper {
        public static bool TryGetActiveProject(DTE2 dte, out Project activeProject) {
            activeProject = null;
            try {
                var activeSolutionProjects = dte.ActiveSolutionProjects as Array;
                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0) {
                    activeProject = activeSolutionProjects.GetValue(0) as Project;
                    return true;
                }
            }
            catch {
                // ignored
            }
            return false;
        }
    }

    public class ProjectNameResolver : SimpleTagResolver {
        public ProjectNameResolver() : base(tagName: "projectName") { }

        public override string Resolve(AvailableInfo info) {
            return GetActiveProjectNameOrEmpty();
        }

        public static string GetActiveProjectNameOrEmpty() {
            return ProjectHelper.TryGetActiveProject(Globals.DTE, out Project project) ? project.Name ?? string.Empty : "";
        }
    }
}