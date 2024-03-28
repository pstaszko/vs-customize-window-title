//using EnvDTE;
//using System.IO;
//using ErwinMayerLabs.RenameVSWindowTitle.Lib;

//namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
//    public class TfsBranchNameResolver : SimpleTagResolver {
//        public TfsBranchNameResolver() : base(tagName: "tfsBranchName") { }

//        public override string Resolve(AvailableInfo info) {
//            return GetBranchNameOrEmpty(info.Solution);
//        }

//        public static string GetBranchNameOrEmpty(Solution solution) {
//            //dynamic vce = Globals.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt");
//            //if (vce != null && vce.SolutionWorkspace != null) {
//            //    return vce.SolutionWorkspace.Name;
//            //}  
//            var sn = solution?.FullName;
//            if (string.IsNullOrEmpty(sn)) return string.Empty;
//            var name = string.Empty;
//            //Globals.InvokeOnUIThread(() => name = TfsHelper.GetBranchNameFromLocalFile(Path.GetDirectoryName(sn))); //Causes slowness due to UI thread.
//            name = TfsHelper.GetBranchNameFromLocalFile(Path.GetDirectoryName(sn));
//            return name ?? string.Empty;
//        }
//    }
//}