using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using MultiMergeShared;

namespace MultiMerge
{


    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidFind_Changeset_By_CommentPkgString)]
    public sealed class MultiMergePackage : AsyncPackage
    {

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public MultiMergePackage()
        {
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            base.Initialize();
            // Switches to the UI thread in order to consume some services used in command initialization
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Query service asynchronously from the UI thread
            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidMerge_Changeset_By_CommentCmdSet, (int)PkgCmdIDList.cmdMultiMerge);
                MenuCommand menuItem = new MenuCommand(MergeByComment, menuCommandID);
                mcs.AddCommand(menuItem);
                CommandID baselessMergeCommandID = new CommandID(GuidList.guid_Merge_ChangeSet_CmdSet, (int)PkgCmdIDList.cmdMergeChangeSet);
                MenuCommand menuItem2 = new MenuCommand(MergeFromHistoryWindow, baselessMergeCommandID);
                mcs.AddCommand(menuItem2);
                CommandID unshelveToBrancheCommandID = new CommandID(GuidList.guid_UnshelveToBranche, (int)PkgCmdIDList.cmdUnshelveToBranch);
                MenuCommand menuItem3 = new MenuCommand(UnShelveToBranch, unshelveToBrancheCommandID);
                mcs.AddCommand(menuItem3);

                //CommandID excludeFromTFS = new CommandID(GuidList.guid_ExcludeFromTFS, (int)PkgCmdIDList.cmdExcludeFromTFS);
                //MenuCommand menuItem3 = new MenuCommand(ExcludeFromTFS, excludeFromTFS);
                //mcs.AddCommand(menuItem3);
            }
        }

        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MergeByComment(object sender, EventArgs e)
        {
            var teamExplorer = GetService(typeof(ITeamExplorer)) as ITeamExplorer;
            // Must be connected to a TFS server to do this
            DTE2 automationObject = (DTE2)GetService(typeof(DTE));
            var versionControl = automationObject.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            var logger = new OutputWindowLogger(this);
            frmMerge findForm = new frmMerge(versionControl, teamExplorer, logger);
            findForm.Show();
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MergeFromHistoryWindow(object sender, EventArgs e)
        {

            // Must be connected to a TFS server to do this
            DTE2 automationObject = (DTE2)GetService(typeof(DTE));

            try
            {
                var logger = new OutputWindowLogger(this);

                VersionControlExt versionControlExt = automationObject.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                //get History window
                var window = versionControlExt?.History?.ActiveWindow;
                if (window != null)
                {
                    var changesets = versionControlExt.History.ActiveWindow.SelectedChangesets;
                    if (changesets != null)
                    {
                        var isServerPath = versionControlExt.History.ActiveWindow.ItemPath.StartsWith("$");
                        string path = isServerPath ? versionControlExt.History.ActiveWindow.ItemPath : versionControlExt.Explorer?.Workspace.GetServerItemForLocalItem(versionControlExt.History.ActiveWindow.ItemPath);
                        if (!string.IsNullOrEmpty(path))
                        {
                            var server = versionControlExt.Explorer?.Workspace?.VersionControlServer;
                            if (server != null)
                            {
                                var teamExplorer = GetService(typeof(ITeamExplorer)) as ITeamExplorer;
                                var form = new MergeUI(versionControlExt, teamExplorer, logger, path, versionControlExt.History.ActiveWindow.SelectedChangesets.Select(c => c.ChangesetId).ToList());
                                form.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var generalPane = GetOutputPane(VSConstants.GUID_OutWindowGeneralPane, "Merge");
                generalPane.OutputString(exception.ToString());
                generalPane.OutputString(Environment.NewLine);
                generalPane.Activate(); // Brings this pane into 
            }

        }

        private void UnShelveToBranch(object sender, EventArgs e)
        {
            try
            {
                var logger = new OutputWindowLogger(this);
                var teamExplorer = GetService(typeof(ITeamExplorer)) as ITeamExplorer;
                //we have to work with private dynamics to get the private/internal properties...
                var model = ((Microsoft.TeamFoundation.Controls.WPF.TeamExplorer.TeamExplorerPageBase)(teamExplorer).NavigateToPage(new Guid(TeamExplorerPageIds.FindShelvesets), (object)null)).Model.AsDynamic();
                var list = model.Shelvesets;
                for (int i = 0; i < list.Count; i++)
                {
                    var isSelected = list[i].IsSelected;
                    if (isSelected)
                    {
                        var item = list[i].Shelveset;
                        var shelveset = PrivateDynamicObject.GetRealInstance(item) as Shelveset;
                        if (shelveset == null)
                        {
                            logger.Error("Failed to get Shelveset details...it was a best guess anyway as these classes are internals of Visual Studio");
                        }
                        else
                        {
                            try
                            {
                                var message = MergeWorker.MergeShelveSet(shelveset, logger);
                                if (!string.IsNullOrEmpty(message))
                                    MessageBox.Show(message);
                            }
                            catch (Exception exception)
                            {
                                logger.Error(exception.Message, exception);
                                MessageBox.Show(exception.Message);
                            }
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                var generalPane = GetOutputPane(VSConstants.GUID_OutWindowGeneralPane, "Merge");
                generalPane.OutputString(exception.ToString());
                generalPane.OutputString(Environment.NewLine);
                generalPane.Activate(); // Brings this pane into 
            }
        }

        private void ExcludeFromTFS(object sender, EventArgs e)
        {
            // Must be connected to a TFS server to do this
            DTE2 automationObject = (DTE2)GetService(typeof(DTE));
            VersionControlExt versionControlExt = automationObject.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            var localpath = versionControlExt.Explorer.CurrentFolderItem.LocalPath;
            if (!string.IsNullOrEmpty(localpath))
            {
                if (Directory.Exists(localpath))
                    versionControlExt.SolutionWorkspace.AddIgnoreFileExclusion(null, localpath);
                else if (File.Exists(localpath))
                    versionControlExt.SolutionWorkspace.AddIgnoreFileExclusion(localpath, null);
            }

        }

        private class OutputWindowLogger : ILogger
        {
            private IVsOutputWindowPane _output;
            public OutputWindowLogger(Package package)
            {
                _output = package.GetOutputPane(VSConstants.GUID_OutWindowGeneralPane, "Merge");
            }

            public void Debug(string message)
            {
                var log = string.Format(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
                _output.OutputString(log);
                _output.OutputString(Environment.NewLine);
            }

            public void Debug(string message, params object[] args)
            {
                var log = string.Format(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, string.Format(message, args));
                _output.OutputString(log);
                _output.OutputString(Environment.NewLine);
            }

            public void Error(string message)
            {
                var log = string.Format(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
                _output.OutputString(log);
                _output.OutputString(Environment.NewLine);
                _output.Activate();
            }

            public void Error(string message, Exception exception)
            {
                var now = DateTime.Now;
                var log = string.Format(@"{0} [{1}] - {2}", now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
                _output.OutputString(log);
                _output.OutputString(Environment.NewLine);
                log = string.Format(@"{0} [{1}] - {2}", now, System.Threading.Thread.CurrentThread.ManagedThreadId, exception);
                _output.OutputString(log);
                _output.OutputString(Environment.NewLine);
                _output.Activate();
            }
        }
    }
}
