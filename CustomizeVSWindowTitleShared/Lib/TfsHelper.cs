///* Requires references to these TFS and Visual Studio assemblies:
// *  Microsoft.TeamFoundation.Client
// *  Microsoft.TeamFoundation.Common
// *  Microsoft.TeamFoundation.VersionControl.Client
// *  Microsoft.TeamFoundation.VersionControl.Common
// *  Microsoft.VisualStudio.Services.Client.Interactive
// *  Microsoft.VisualStudio.Services.Common
// * You'll find them in the TF.exe path (for VS2017 community edition: Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer)
// * For each of these references, set "Specific version" and "Local copy" to false, because you want to use whatever comes with Visual Studio at runtime
// */

//using System;
//using Microsoft.TeamFoundation.Client;
//using Microsoft.TeamFoundation.VersionControl.Client;
//using Microsoft.TeamFoundation.VersionControl.Common;
//using Microsoft.VisualStudio.Services.Client;

//namespace ErwinMayerLabs.RenameVSWindowTitle.Lib {
//    internal static class TfsHelper {
//        public static string GetBranchNameFromLocalFile(string path) {
//            if (string.IsNullOrEmpty(path))
//                return null;
//            var url = GetRepositoryUrl(path);
//            if (string.IsNullOrEmpty(url))
//                return null;
//            var col = CreateTfsCollection(url);
//            var tfs = GetVersionControlServer(col);
//            var branch = FindBranch(tfs, path);
//            return System.IO.Path.GetFileName(branch);
//        }

//        public static string FindBranch(VersionControlServer tfs, string path, VersionSpec versionSpec = null) {
//            if (versionSpec == null)
//                versionSpec = VersionSpec.Latest;
//            try {
//                while (!string.IsNullOrEmpty(path)) {
//                    var item = tfs.GetItem(path, versionSpec, DeletedState.NonDeleted, GetItemsOptions.IncludeBranchInfo);
//                    if (item == null)
//                        return null;
//                    if (item.IsBranch)
//                        return path;
//                    path = System.IO.Path.GetDirectoryName(path);
//                }
//                return null;
//            }
//            catch (ItemNotMappedException) {
//                return null;
//            }
//        }

//        public static VersionControlServer GetVersionControlServer(TfsTeamProjectCollection collection) {
//            return collection.GetService<VersionControlServer>();
//        }

//        public static TfsTeamProjectCollection CreateTfsCollection(string tfsName) {
//            //This function may be a bit slow and runs on the UI thread, so would deserve optimization if many people are using it.
//            //Works for VS 2017+, with assemblies from VS 15. Does not work when compiling the VSIX from VS 2015 (will cause an exception saying the assembly could not be found).
//            Uri fullyQualifiedUriForName = TfsTeamProjectCollection.GetFullyQualifiedUriForName(tfsName);
//            if (Resolvers.IdeHelper.VsMajorVersion < 17) { 
//                var vssCredentials = VssClientCredentials.LoadCachedCredentials(fullyQualifiedUriForName, false, Microsoft.VisualStudio.Services.Common.CredentialPromptType.DoNotPrompt);
//                var tfsTeamProjectCollection = new TfsTeamProjectCollection(fullyQualifiedUriForName, vssCredentials);
//                return tfsTeamProjectCollection;
//            }
//            //Only works with VS2022+
//            var vssCredentials2 = new VssClientCredentials(useDefaultCredentials: true);
//            var tfsTeamProjectCollection2 = new TfsTeamProjectCollection(fullyQualifiedUriForName, vssCredentials2);
//            tfsTeamProjectCollection2.Authenticate(); //Will only prompt user to authenticate once normally.
//            return tfsTeamProjectCollection2;
//        }

//        public static string GetRepositoryUrl(string path) {
//            string text = null;
//            if (!VersionControlPath.IsServerItem(path)) {
//                try {
//                    text = GetWorkspace(path)?.ServerUri?.AbsoluteUri;
//                }
//                catch (ApplicationException) {
//                }
//            }
//            return text;
//        }

//        private static WorkspaceInfo GetWorkspace(string path) {
//            if (path != null) {
//                try {
//                    if (!VersionControlPath.IsServerItem(path) && Workstation.Current.IsMapped(path)) {
//                        return Workstation.Current.GetLocalWorkspaceInfo(path);
//                    }
//                }
//                catch (Microsoft.TeamFoundation.InvalidPathException) {
//                }
//            }
//            return null;
//        }

//    }
//}