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
			var pts = new ParameterizedThreadStart(obj => VSocketQQ.Listen(Globals.DTE));
			var tt = new System.Threading.Thread(pts);
			tt.Start();
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			await Command1.InitializeAsync(this);

		}

		protected override void Initialize()
		{
			var pts = new ParameterizedThreadStart(obj => VSocketQQ.Listen(Globals.DTE));
			var tt = new System.Threading.Thread(pts);
			tt.Start();
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			//await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			//await Command1.InitializeAsync(this);
			base.Initialize();
		}
		#endregion
	}
}
