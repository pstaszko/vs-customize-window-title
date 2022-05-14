using EnvDTE;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class WorkspaceNameResolver : SimpleTagResolver {
        public WorkspaceNameResolver() : base(tagName: "workspaceName") { }

        public override string Resolve(AvailableInfo info) {
            return GetWorkspaceNameOrEmpty(info.Solution);
        }

        public static string GetWorkspaceNameOrEmpty(Solution solution) {
            //dynamic vce = Globals.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt");
            //if (vce != null && vce.SolutionWorkspace != null) {
            //    return vce.SolutionWorkspace.Name;
            //}  
            var sn = solution?.FullName;
            if (string.IsNullOrEmpty(sn)) return string.Empty;
            var name = string.Empty;
            Globals.InvokeOnUIThread(() => name = WorkspaceInfoGetter.Instance().GetName(sn));
            return name ?? string.Empty;
        }
    }

    public class WorkspaceOwnerNameResolver : SimpleTagResolver {
        public WorkspaceOwnerNameResolver() : base(tagName: "workspaceOwnerName") { }

        public override string Resolve(AvailableInfo info) {
            return GetWorkspaceOwnerNameOrEmpty(info.Solution);
        }

        public static string GetWorkspaceOwnerNameOrEmpty(Solution solution) {
            //dynamic vce = Globals.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt");
            //if (vce != null && vce.SolutionWorkspace != null) {
            //    return vce.SolutionWorkspace.OwnerName;
            //}  
            var sn = solution?.FullName;
            if (string.IsNullOrEmpty(sn)) return string.Empty;
            var name = string.Empty;
            Globals.InvokeOnUIThread(() => name = WorkspaceInfoGetter.Instance().GetOwner(sn));
            return name ?? string.Empty;
        }

    }
}