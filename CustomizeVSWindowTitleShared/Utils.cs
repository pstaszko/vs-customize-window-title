using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using EnvDTE;                               //DTE
using EnvDTE80;
//using Microsoft.VisualStudio.Project.VisualC.VCProjectEngine;
//using Microsoft.VisualStudio.VCProjectEngine;
using System.Runtime.InteropServices;
using System.Text;
using System.Management;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading;
using EnvDTE90;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.Project.VisualC.VsShell.Interop;
using VSLangProj;
using Microsoft.VisualStudio.Shell;

namespace CustomizeVSWindowTitleShared
{
	public static class Utils
	{
		private static Guid FOLDER = Guid.Parse("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
		static private void FindProjectsIn(EnvDTE.ProjectItem item, List<EnvDTE.Project> results)
		{
			if (item.Object is EnvDTE.Project)
			{
				var proj = (EnvDTE.Project)item.Object;
				if (new Guid(proj.Kind) != FOLDER)
				{
					results.Add((EnvDTE.Project)item.Object);
				}
				else
				{
					foreach (EnvDTE.ProjectItem innerItem in proj.ProjectItems)
					{
						FindProjectsIn(innerItem, results);
					}
				}
			}
			if (item.ProjectItems != null)
			{
				foreach (EnvDTE.ProjectItem innerItem in item.ProjectItems)
				{
					FindProjectsIn(innerItem, results);
				}
			}
		}

		static private void FindProjectsIn(EnvDTE.UIHierarchyItem item, List<EnvDTE.Project> results)
		{
			if (item.Object is EnvDTE.Project)
			{
				var proj = (EnvDTE.Project)item.Object;
				if (new Guid(proj.Kind) != FOLDER)
				{
					results.Add((EnvDTE.Project)item.Object);
				}
				else
				{
					foreach (EnvDTE.ProjectItem innerItem in proj.ProjectItems)
					{
						FindProjectsIn(innerItem, results);
					}
				}
			}
			foreach (EnvDTE.UIHierarchyItem innerItem in item.UIHierarchyItems)
			{
				FindProjectsIn(innerItem, results);
			}
		}

		static internal IEnumerable<EnvDTE.Project> GetEnvDTEProjectsInSolution()
		{
			List<EnvDTE.Project> ret = new List<EnvDTE.Project>();
			var dte = (EnvDTE80.DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
			if (dte != null)
			{
				EnvDTE.UIHierarchy hierarchy = dte.ToolWindows.SolutionExplorer;
				foreach (EnvDTE.UIHierarchyItem innerItem in hierarchy.UIHierarchyItems)
				{
					FindProjectsIn(innerItem, ret);
				}
			}
			return ret;
		}
	}
}
