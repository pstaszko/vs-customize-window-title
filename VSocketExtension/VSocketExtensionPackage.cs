using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using VSocketExtension;
using Task = System.Threading.Tasks.Task;

namespace VSocketExtension
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(VSocketExtensionPackage.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	//[ProvideAutoLoad(UIContextGuids80.SolutionExists)]
	//[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	//[ProvideAutoLoad(UIContextGuids80.EmptySolution)]
	[ProvideAutoLoad(UIContextGuids80.SolutionBuilding)]
	[ProvideAutoLoad(UIContextGuids80.ToolboxInitialized)]
	[ProvideAutoLoad(UIContextGuids80.WindowsFormsDesigner)]
	[ProvideAutoLoad(UIContextGuids80.DataSourceWindowAutoVisible)]
	[ProvideAutoLoad(UIContextGuids80.DataSourceWindowSupported)]
	[ProvideAutoLoad(UIContextGuids80.SolutionOrProjectUpgrading)]
	[ProvideAutoLoad(UIContextGuids80.NotBuildingAndNotDebugging)]
	[ProvideAutoLoad(UIContextGuids80.CodeWindow)]
	[ProvideAutoLoad(UIContextGuids80.SolutionExistsAndNotBuildingAndNotDebugging)]
	[ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects)]
	[ProvideAutoLoad(UIContextGuids80.EmptySolution)]
	[ProvideAutoLoad(UIContextGuids80.SolutionExists)]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	[ProvideAutoLoad(UIContextGuids80.DesignMode)]
	[ProvideAutoLoad(UIContextGuids80.FullScreenMode)]
	[ProvideAutoLoad(UIContextGuids80.Dragging)]
	[ProvideAutoLoad(UIContextGuids80.Debugging)]
	[ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject)]
	[ProvideAutoLoad(UIContextGuids.SolutionBuilding)]
	[ProvideAutoLoad(UIContextGuids.Debugging)]
	[ProvideAutoLoad(UIContextGuids.Dragging)]
	[ProvideAutoLoad(UIContextGuids.FullScreenMode)]
	[ProvideAutoLoad(UIContextGuids.DesignMode)]
	[ProvideAutoLoad(UIContextGuids.NoSolution)]
	[ProvideAutoLoad(UIContextGuids.SolutionExists)]
	[ProvideAutoLoad(UIContextGuids.EmptySolution)]
	[ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject)]
	[ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects)]
	[ProvideAutoLoad(UIContextGuids.CodeWindow)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.RESXEditor_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.FSharpProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VCProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.CSharpProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ToolboxChooseItemsDataSourceInitialized_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ToolboxInitialized_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ProjectCreating_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOrProjectUpgrading_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.BulkFileOperation_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.FullSolutionLoading_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.OutputWindowCreated_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasAppContainerProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndNotBuildingAndNotDebugging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionBuilding_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.RepositoryOpen_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeAttribute_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeClass_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeDelegate_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeEnum_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.CloudEnvironmentConnected_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ToolWindowActive_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.DocumentWindowActive_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.FolderOpened_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SharedMSBuildFilesManagerHierarchyLoaded_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SynchronousSolutionOperation_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasWindowsPhone80NativeProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.WizardOpen_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ProjectRetargeting_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSilverlightWindowsPhoneProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.StandardPreviewerConfigurationChanging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.BackgroundProjectLoad_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeVariable_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeStruct_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeProperty_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeParameter_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeNamespace_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeInterface_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBCodeFunction_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.IdeUserSignedIn_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ToolboxVisible_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.DesignMode_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.DataSourceWindowAutoVisible_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.DataSourceWizardSuppressed_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.MainToolBarVisible_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.FullScreenMode_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.CodeWindow_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.MainToolBarInvisible_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.VBProjOpened_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.HistoricalDebugging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.FirstLaunchSetup_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.DataSourceWindowSupported_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.ApplicationDesigner_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.CloudDebugging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.MinimalMode_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.PropertyPageDesigner_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NotBuildingAndNotDebugging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.OsWindows8OrHigher_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionClosing_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.Dragging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.EmptySolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.Debugging_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SettingsDesigner_string)]
	//[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
	//[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
	//[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
	//[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
	public sealed class VSocketExtensionPackage : AsyncPackage
	{
		public VSocketExtensionPackage()
		{
		}
		/// <summary>
		/// VSocketExtensionPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "0d39ce63-0962-4e6c-bf1b-a2385a994683";

		#region Package Members
		//protected override System.Threading.Tasks.Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
		//{
		//	System.IO.File.WriteAllText(@"C:\Dev\temp\x.txt", "a");


		//	return base.InitializeToolWindowAsync(toolWindowType, id, cancellationToken);
		//}
		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			//this.QueryService
			//VSocketQQ.Start();
			var pts = new ParameterizedThreadStart(obj => VSocketQQ.Listen(Globals.DTE));
			var tt = new System.Threading.Thread(pts);
			tt.Start();
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			//await Command1.InitializeAsync(this);
		}

		//protected override void Initialize()
		//{
		//VSocketQQ.Start();
		//	// When initialized asynchronously, the current thread may be a background thread at this point.
		//	// Do any initialization that requires the UI thread after switching to the UI thread.
		//	//await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		//	//await Command1.InitializeAsync(this);
		//	base.Initialize();
		//}
		#endregion
	}
}
