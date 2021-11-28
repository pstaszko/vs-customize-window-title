# Customize Visual Studio Window Title - VS Extension
![Screenshot](/CustomizeVSWindowTitleSharedFolder/Screenshot.png?raw=true "Screenshot")

This is the official repository for:  
https://marketplace.visualstudio.com/items?itemName=mayerwin.RenameVisualStudioWindowTitle

Initial release page here:  
http://erwinmayer.com/labs/visual-studio-2010-extension-rename-visual-studio-window-title

**Description**

This lightweight extension allows changing the window title of Visual Studio to include a folder tree with a configurable min depth and max depth distance from the solution/project file, and the use of special tags to help with many other possible scenarios (Git, Mercurial, SVN, TFS...). 

Solution-specific overriding rules are available as well to cover virtually any possible renaming needs.

It can also be configured so that the rules apply only when at least two instances of Visual Studio are running with the same window title.

**Documentation**

Full documentation is available on the [Visual Studio Marketplace page](https://marketplace.visualstudio.com/items?itemName=mayerwin.RenameVisualStudioWindowTitle).

**Supported editions**

Visual Studio 2015, 2017, 2019, 2022+.

Visual Studio 2022 support is currently available via the beta release which can be downloaded [here](https://github.com/mayerwin/vs-customize-window-title/releases/tag/5.0.1). The extension available on the Visual Studio Marketplace will be updated to support Visual Studio 2022 when Microsoft will have actually implemented the following key important missing feature:

> In the future, the Marketplace will allow you to upload multiple VSIXs to just one Marketplace listing, allowing you to upload your Visual Studio 2022-targeted VSIX and a pre-Visual Studio 2022 VSIX. Your users will automatically get the right VSIX for the VS version they have installed, when using the VS extension manager.
https://docs.microsoft.com/en-us/visualstudio/extensibility/migration/update-visual-studio-extension?view=vs-2022#visual-studio-marketplace

Visual Studio 2010 support has been dropped due to limitations imposed by Microsoft's VSIX format to support Visual Studio 2017. The last stable release supporting Visual Studio 2010 can however be downloaded [here](https://github.com/mayerwin/vs-customize-window-title/releases/tag/3.3.6).

Visual Studio 2012 and 2013 support has been dropped due to new requirements imposed by Microsoft to speed up loading time with AsyncPackage. The last stable release supporting Visual Studio 2012 and 2013 can however be downloaded [here](https://github.com/mayerwin/vs-customize-window-title/releases/tag/3.8.1).

You are welcome to contribute to this project!