using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Shell.Interop;
using MultiMergeShared;

namespace MultiMerge
{
    public class MergeWorker
    {
        private readonly Workspace _workspace;
        private readonly VersionControlServer _versionControlServer;
        private readonly ITeamExplorer _teamExplorer;
        private readonly ILogger _logger;

        public event Action<Changeset> ChangeSetAdded;
        public event Action<List<string>> FilesAdded;
        public event Action<Dictionary<string, List<Changeset>>> MissingChangesForFileDetected;
        public event Action<int, string> MergeProgressChanged;

        /// <summary>
        /// The list of changesets returned from the search
        /// </summary>
        private HashSet<Changeset> _changes = new HashSet<Changeset>();

        /// <summary>
        /// The list of affected _files
        /// </summary>
        private Dictionary<string, HashSet<Changeset>> _files = new Dictionary<string, HashSet<Changeset>>();

        public MergeWorker(Workspace workspace, VersionControlServer versionControlServer, ITeamExplorer teamExplorer, ILogger logger)
        {
            _workspace = workspace;
            _versionControlServer = versionControlServer;
            _teamExplorer = teamExplorer;
            _logger = logger;
        }

        public int FileCount
        {
            get { return _files.Count; }
        }

        public Dictionary<string, int> FileCountPerChangeSet()
        {
            return _changes.ToDictionary(c => c.ChangesetId.ToString(), c => _files.Count(pv => pv.Value.Contains(c)));
        }

        public int ChangesetCount
        {
            get { return _changes.Count; }
        }

        public Tuple<int, int> AnalyseChangeSet(Changeset changeset)
        {
            if (!_changes.Any(c => c.ChangesetId == changeset.ChangesetId))
            {
                this._changes.Add(changeset);
                ChangeSetAdded?.Invoke(changeset);
            }

            var filesAdded = new List<string>();
            foreach (Change change in changeset.Changes)
            {
                HashSet<Changeset> ch;
                var fileName = change.Item.ServerItem;
                if (!this._files.TryGetValue(fileName, out ch))
                {
                    ch = new HashSet<Changeset>();
                    this._files.Add(fileName, ch);
                    filesAdded.Add(fileName);
                }
                if (!ch.Any(c => c.ChangesetId == changeset.ChangesetId))
                {
                    ch.Add(changeset);
                }
            }
            FilesAdded?.Invoke(filesAdded);
            return new Tuple<int, int>(_files.Count, _changes.Count);
        }


        public void DetectMissingChangeSets(bool addRelevantChangesets)
        {
            var missingChangeSets = new Dictionary<int, Changeset>();
            var notifyChanges = new Dictionary<string, List<Changeset>>();
            foreach (var file in _files.Keys.Select(k => k).ToList())
            {
                var missingforFile = GetMissingChangesets(file);
                if (addRelevantChangesets)
                {
                    foreach (var missingChangeset in missingforFile)
                    {
                        if (!missingChangeSets.ContainsKey(missingChangeset.ChangesetId))
                            missingChangeSets.Add(missingChangeset.ChangesetId, missingChangeset);
                    }
                }
                else
                {
                    notifyChanges[file] = missingforFile;
                }
            }

            if (addRelevantChangesets)
            {
                do
                {
                    missingChangeSets = new Dictionary<int, Changeset>(AddRelevantFilesForMissingChangesets(missingChangeSets));
                } while (missingChangeSets.Any());
            }
            else
            {
                MissingChangesForFileDetected?.Invoke(notifyChanges);
            }
        }

        private List<Changeset> GetMissingChangesets(string file)
        {
            HashSet<Changeset> ch;
            var list = new List<Changeset>();
            if (this._files.TryGetValue(file, out ch))
            {
                if (ch.Count > 1)
                {
                    var changesets = new List<Changeset>(ch).OrderBy(c => c.ChangesetId).DistinctBy(c => c.ChangesetId).ToList();
                    var history = _versionControlServer.QueryHistory(file, RecursionType.Full).OrderBy(c => c.ChangesetId).ToList();
                    for (int i = 0; i < changesets.Count - 1; i++)
                    {
                        var changeset = changesets[i];
                        var index = history.FindIndex(c => c.ChangesetId == changeset.ChangesetId);
                        var nextmissingChangeset = history[index + 1];
                        var nextFoundchangeset = changesets[i + 1];
                        if (nextmissingChangeset.ChangesetId != nextFoundchangeset.ChangesetId)
                        {
                            ch.Add(nextmissingChangeset);
                            list.Add(nextmissingChangeset);
                        }
                    }
                }
            }
            return list;
        }


        private Dictionary<int, Changeset> AddRelevantFilesForMissingChangesets(Dictionary<int, Changeset> missingchangesets)
        {
            var notifyChanges = new Dictionary<string, List<Changeset>>();
            foreach (var missingchangeset in missingchangesets.Values)
            {
                AnalyseChangeSet(missingchangeset);
            }
            var list = new Dictionary<int, Changeset>();
            foreach (var file in _files.Keys.Select(k => k).ToList())
            {
                var missingforFile = GetMissingChangesets(file);
                foreach (var changeset in missingforFile)
                {
                    list[changeset.ChangesetId] = changeset;
                }
                notifyChanges[file] = missingforFile;
            }
            MissingChangesForFileDetected?.Invoke(notifyChanges);
            return list;
        }

        public string GetEligibleMergePath(string basePath, out bool baselessMerge)
        {
            var paths = PathEligibleForMerge(_versionControlServer, basePath);
            return AskUserForTargetBranch(_versionControlServer, paths, out baselessMerge);
        }

        private static Workspace AskUserForWorkspace(List<Workspace> workspaces)
        {
            string targetWorkspaceName = null;
            List<string> workspaceNames = workspaces.Select(x => x.Name).ToList();
            if (InputDialog.Show("Please enter target workspace", "Choose target workspace", workspaceNames, ref targetWorkspaceName) == DialogResult.OK)
            {
                if (!workspaceNames.Contains(targetWorkspaceName))
                {
                    MessageBox.Show($"{targetWorkspaceName} is not a valid TFS workspace", "Multi Merge", MessageBoxIcon.Exclamation);
                    targetWorkspaceName = string.Empty;
                }
            }
            else
            {
                targetWorkspaceName = string.Empty;
            }

            return workspaces.FirstOrDefault(x => x.Name == (targetWorkspaceName ?? string.Empty));
        }

        private static string AskUserForTargetBranch(VersionControlServer vcs, List<string> paths, out bool baselessMerge)
        {
            string targetBranch = null;
            baselessMerge = false;
            if (InputDialog.Show("Please enter target branch", "Choose target branch", paths, ref targetBranch) ==
                DialogResult.OK)
            {
                baselessMerge = !paths.Contains(targetBranch);
                if (!vcs.ServerItemExists(targetBranch, ItemType.Any))
                {
                    MessageBox.Show(string.Format("{0} is not a valid TFS path", targetBranch), "Multi Merge",
                        MessageBoxIcon.Exclamation);
                    targetBranch = string.Empty;
                }
            }
            else
            {
                targetBranch = string.Empty;
            }

            return targetBranch ?? string.Empty;
        }

        public IEnumerable<string> MergeChanges(MergeParameters config, BackgroundWorker worker)
        {
            string basePath = config.BasePath;
            string targetBranch = config.TargetPath;
            bool baselessMerge = config.BaselessMerge;
            bool mergeCompleteFileHistory = config.MergeToLatest;
            int mergeConflicts = _workspace.QueryConflicts(new string[] { targetBranch }, true).Length;
            string source = basePath;
            var expectedMergedFileNames = new HashSet<string>(GetFilesToMerge(config.ExcludedFiles).Select(f => f.Key.Replace(source, targetBranch)));
            var pendingChanges = new HashSet<string>(_workspace.GetPendingChanges(targetBranch, RecursionType.Full, false).Select(p => p.ServerItem));
            var conflictPending = pendingChanges.Where(p => expectedMergedFileNames.Any(e => e.EndsWith(p.Replace("$", "")))).Select(s => $"- {s}").ToList();
            var errors = new List<string>();
            if (conflictPending.Any())
                errors.Add($"Cannot merge because {(conflictPending.Count == 1 ? "this file is" : "these files are")} already checked out: {Environment.NewLine}{string.Join(Environment.NewLine, conflictPending)}");
            else
            {
                //special case...we let VS do the changeset merge by itself...
                //since we do want to provide cancellation functionality, we wrap it in a separate process
                if (_changes.Count == 1 && config.ExcludedFiles.Count == 0)
                {
                    var waitHandle = new ManualResetEvent(false);
                    Thread t = new Thread(() =>
                    {
                        try
                        {
                            var mergeItem = new MergeItem
                            { Changesets = _changes.ToList(), DestinationPath = targetBranch, SourcePath = basePath };
                            var status = MergeItem(mergeItem, baselessMerge, mergeCompleteFileHistory);
                            var failures = status.GetFailures();
                            errors.AddRange(failures.Select(f =>
                                $"Failed merging changeset '{_changes.First().ChangesetId}' to '{targetBranch}' with Error Message: {f.Message}"));
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Merge failed: {e.Message}");
                        }
                        finally
                        {
                            waitHandle.Set();
                        }
                    });
                    t.Start();
                    while (!waitHandle.WaitOne(1000))//poll every second if user cancelled
                    {
                        if (worker.CancellationPending)
                            t.Abort();
                        else
                        {
                            MergeProgressChanged?.Invoke(0, $"Performing actual merge...");
                        }
                    }
                }
                else
                {
                    var allMerges = GetMergeItems(basePath, targetBranch, config.ExcludedFiles, ref config.CancelAnalysis);
                    var folderMerges = allMerges.Where(m => m.IsFolder).OrderBy(m => m.SourcePath).ToList();
                    var fileMerges = allMerges.Where(m => !m.IsFolder);
                    var finalFileMerge = new List<MergeItem>();
                    var finalFolderMerges = new List<MergeItem>();

                    foreach (var fileMerge in fileMerges)
                    {
                        var folderMerge = folderMerges.FirstOrDefault(m =>
                            fileMerge.SourcePath.StartsWith(m.SourcePath + "/"));
                        if (folderMerge != null)
                        {
                            var newChangesets = fileMerge.Changesets.Where(c1 =>
                                folderMerge.Changesets.All(c2 => c2.ChangesetId != c1.ChangesetId));
                            folderMerge.Changesets.AddRange(newChangesets);
                        }
                        else
                            finalFileMerge.Add(fileMerge);
                    }

                    foreach (var folderMerge in folderMerges)
                    {
                        var finalfolderMerge =
                            finalFolderMerges.FirstOrDefault(m => m.SourcePath.StartsWith(folderMerge.SourcePath));
                        if (finalfolderMerge != null)
                        {
                            var newChangesets = folderMerge.Changesets.Where(c1 =>
                                finalfolderMerge.Changesets.All(c2 => c2.ChangesetId != c1.ChangesetId));
                            finalfolderMerge.Changesets.AddRange(newChangesets);
                        }
                        else
                            finalFolderMerges.Add(folderMerge);
                    }

                    double maximum = finalFileMerge.Count + finalFolderMerges.Count;
                    var merged = 0;
                    MergeProgressChanged?.Invoke(30, $"Performing actual merge...(0 of {maximum})");
                    foreach (var mergeItem in finalFileMerge)
                    {
                        try
                        {
                            merged += 1;
                            if (worker.CancellationPending)
                                throw new Exception("User canceled operation");
                            var status = MergeItem(mergeItem, baselessMerge, mergeCompleteFileHistory);

                            var faillures = status.GetFailures();
                            errors.AddRange(faillures.Select(f =>
                                $"Failed merging file '{mergeItem.SourcePath}' with Error Message: {f.Message}"));
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Failed merging file '{mergeItem.SourcePath}' with Error Message: {e.Message}");
                            if (worker.CancellationPending)
                                return errors;
                        }
                        //we start at 30%
                        int percentage = (int)(30 + (merged / maximum) * 70);
                        MergeProgressChanged?.Invoke(percentage, $"Performing actual merge...({merged} of {maximum})");
                    }

                    foreach (var mergeItem in finalFolderMerges)
                    {
                        merged += 1;
                        try
                        {
                            var status = MergeItem(mergeItem, baselessMerge, mergeCompleteFileHistory);
                            var faillures = status.GetFailures();
                            errors.AddRange(faillures.Select(f =>
                                $"Failed merging folder '{mergeItem.SourcePath}' with Error Message: {f.Message}"));
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Failed merging folder '{mergeItem.SourcePath}' with Error Message: {e.Message}");
                        }
                        int percentage = (int)((merged / maximum) * 70);
                        MergeProgressChanged?.Invoke(percentage, $"Performing actual merge...({merged} of {maximum})");
                    }
                }
                var updatedpendingChanges = new HashSet<string>(_workspace.GetPendingChanges(targetBranch, RecursionType.Full, false).Select(p => p.ServerItem));
                expectedMergedFileNames.UnionWith(pendingChanges);
                updatedpendingChanges.RemoveWhere(expectedMergedFileNames.Contains);
                if (updatedpendingChanges.Any())
                {
                    _logger.Debug("Unintended merged files found, due to intersecting ChangeSets.");
                    foreach (var unexpectedChange in updatedpendingChanges)
                    {
                        if (!config.MergeIntersectingFiles)
                        {
                            _logger.Debug($"Undo pending merge of file: {unexpectedChange}");
                            _workspace.Undo(unexpectedChange);
                        }
                        else
                        {
                            _logger.Debug($"Merge includes intersected file: {unexpectedChange}");
                        }
                    }
                }


                _logger.Debug("Linking WorkItems");
                worker.ReportProgress(99, new Action(() => AddWorkItems(_changes.SelectMany(c => c.AssociatedWorkItems).Select(w => w.Id).Distinct().ToList())));

                var conflicts = _workspace.QueryConflicts(new string[] { targetBranch }, true).Length;
                conflicts -= mergeConflicts;
                if (conflicts > 0)
                {
                    MergeProgressChanged(100, $"Merge succeeded with {conflicts} conflicts. Please resolve conflicts manually.");
                }
                else
                {
                    MergeProgressChanged(100, $"Merge succeeded without any errors.");
                }
            }
            return errors;
        }

        private void AddWorkItems(List<int> workItemIds)
        {
            if (workItemIds == null || !workItemIds.Any())
                return;
            if (_teamExplorer != null)
            {
                try
                {
                    //TFS API does not support adding related work items to pending changes. So we mimic a user adding it at the pending Changes-page manually, using reflection
                    //if the work item's already listed, it won't cause an issue, so we can simply add duplicate id's
                    IPendingCheckin model = ((TeamExplorerPageBase)(_teamExplorer).NavigateToPage(new Guid(TeamExplorerPageIds.PendingChanges), (object)null)).Model as IPendingCheckin;
                    if (model != null)
                    {

                        MethodInfo method = ((object)model).GetType().GetMethod("AddWorkItemsByIdAsync", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        int[] array = workItemIds.ToArray();

                        object[] parameters = new object[2]
                        {
                            (object) array,
                            (object) 1
                        };
                        method.Invoke((object)model, parameters);

                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to associate pending changes to relevant TFS items", ex);
                }
            }
            else { _logger.Debug("Failed to associate pending changes to relevant TFS items because ITeamExplorer is null"); }
        }

        private GetStatus MergeItem(MergeItem mergeItem, bool baseLess, bool completeChangeHistory)
        {
            var minChangesetId = mergeItem.Changesets.Min(c => c.ChangesetId);
            var maxChangesetId = mergeItem.Changesets.Max(c => c.ChangesetId);
            ChangesetVersionSpec min = null;
            ChangesetVersionSpec max = null;
            if (!completeChangeHistory)
            {
                min = new ChangesetVersionSpec(minChangesetId);
                max = new ChangesetVersionSpec(maxChangesetId);
            }
            var mergeOption = baseLess ? MergeOptionsEx.Baseless : MergeOptionsEx.None;
            var status = _workspace.Merge(mergeItem.SourcePath,
                mergeItem.DestinationPath, min, max, LockLevel.None, RecursionType.Full, mergeOption);
            return status;
        }

        IEnumerable<KeyValuePair<string, HashSet<Changeset>>> GetFilesToMerge(IEnumerable<string> excludedFiles)
        {
            foreach(var file in _files)
            {
                if (!excludedFiles.Any(e => string.Compare(file.Key, e, true) == 0))
                {
                    yield return file;
                }
            }
        }

        private List<MergeItem> GetMergeItems(string basePath, string targetBranch, IEnumerable<string> excludedFiles, ref bool cancel)
        {
            var allMerges = new List<MergeItem>();
            //we start at 30%
            int analized = 0;
            double maximum = _files.Count;
            string progress = "Analyzing file merge: ({0} of {1})";
            MergeProgressChanged?.Invoke(0, string.Format(progress, 0, maximum));
            foreach (var pair in GetFilesToMerge(excludedFiles))
            {
                string from, to;
                bool isFolder;
                GetMergePathsForFile(basePath, pair.Key, targetBranch, ref cancel, out isFolder, out @from, out to);
                MergeItem mergeItem = new MergeItem();
                mergeItem.SourcePath = @from;
                mergeItem.DestinationPath = to;
                mergeItem.IsFolder = isFolder;
                mergeItem.Changesets.AddRange(pair.Value);
                allMerges.Add(mergeItem);
                analized += 1;
                //we start at 30%
                int percentage = (int)((analized / maximum) * 30);
                MergeProgressChanged?.Invoke(percentage, string.Format(progress, analized, maximum));
            }
            return allMerges;
        }

        private HashSet<Changeset> GetMissingChangesetsForFolder(MergeItem folder)
        {
            HashSet<Changeset> ch = new HashSet<Changeset>();

            var changesets = new List<Changeset>(folder.Changesets).OrderBy(c => c.ChangesetId).DistinctBy(c => c.ChangesetId).ToList();
            var history = _versionControlServer.QueryHistory(folder.SourcePath, RecursionType.Full).OrderBy(c => c.ChangesetId).ToList();
            for (int i = 0; i < changesets.Count - 1; i++)
            {
                var changeset = changesets[i];
                var index = history.FindIndex(c => c.ChangesetId == changeset.ChangesetId);
                var nextmissingChangeset = history[index + 1];
                var nextFoundchangeset = changesets[i + 1];
                if (nextmissingChangeset.ChangesetId != nextFoundchangeset.ChangesetId)
                {
                    ch.Add(nextmissingChangeset);
                }
            }
            return ch;
        }

        private void GetMergePathsForFile(string basePath, string sourcePath, string targetbranch, ref bool cancel, out bool isFolder, out string @from, out string to)
        {
            isFolder = false;
            @from = sourcePath;
            var destinationPath = @from.Replace(basePath, targetbranch);
            to = destinationPath;
            if (!cancel)
            {
                if (!@_versionControlServer.ServerItemExists(sourcePath, ItemType.Any))//file deleted
                {
                    isFolder = true;
                    var maxpaths = basePath.Split('/').Count();
                    var subPaths = sourcePath.Split('/').ToList();
                    do
                    {
                        subPaths.RemoveAt(subPaths.Count - 1);
                        @from = string.Join("/", subPaths);
                        to = @from.Replace(basePath, targetbranch);
                    } while (subPaths.Count >= maxpaths && !_versionControlServer.ServerItemExists(@from, ItemType.Folder));
                }
                else if (!_versionControlServer.ServerItemExists(destinationPath, ItemType.Any))// file added
                {
                    isFolder = true;
                    var maxpaths = basePath.Split('/').Count();
                    var subPaths = sourcePath.Split('/').ToList();
                    do
                    {
                        subPaths.RemoveAt(subPaths.Count - 1);
                        @from = string.Join("/", subPaths);
                        to = @from.Replace(basePath, targetbranch);
                    } while (subPaths.Count >= maxpaths && !_versionControlServer.ServerItemExists(to, ItemType.Folder));
                }
            }
        }

        internal static List<string> PathEligibleForMerge(VersionControlServer versionControlServer, string path)
        {

            List<string> result = new List<string>();
            foreach (ItemIdentifier mergeItem in versionControlServer.QueryMergeRelationships(path))
            {
                if (!mergeItem.IsDeleted && !string.IsNullOrWhiteSpace(mergeItem.Item))
                {
                    result.Add(mergeItem.Item);
                }
            }
            return result.OrderBy(x => x).ToList();
        }

        public class MergeParameters
        {
            public string BasePath;
            public string TargetPath;
            public bool BaselessMerge;
            public bool MergeToLatest;
            public bool CancelAnalysis;
            public List<string> ExcludedFiles = new List<string>();
            public bool MergeIntersectingFiles;
        }

        public static string MergeShelveSet(Shelveset shelveset, ILogger logger)
        {
            VersionControlServer vcs = shelveset.VersionControlServer;
            var changes = vcs.QueryShelvedChanges(shelveset);
            var pending = changes.SelectMany(c => c.PendingChanges);
            string fromBranch;
            List<string> targetBranches;
            Workspace workspace = GetWorkSpace(vcs, pending, logger, out fromBranch, out targetBranches);
            if (workspace == null)
                throw new Exception("Could not determine workspace!");
            logger.Debug($"Found source branch: {fromBranch}");
            var otherBranches = pending.Where(p => !p.ServerItem.StartsWith(fromBranch));
            if (otherBranches.Any())
            {
                logger.Debug($"Detected item with different path from source branch: {otherBranches.First().ServerItem}");
                throw new Exception("Shelveset contains items from multiple branches. This is not supported!");
            }
            var targetBranch = AskUserForTargetBranch(vcs, targetBranches, out var baseless);
            if (!string.IsNullOrEmpty(targetBranch))
            {
                var queue = new UpdateLocalVersionQueue(workspace);
                var shelveMerger = new ShelvesetWorker();
                return shelveMerger.UnshelveMerge(shelveset.Name, shelveset.OwnerName, pending.ToArray(), new MigrationRule(fromBranch, targetBranch), workspace, logger, queue);
            }
            logger.Debug("Cancel unshelve: user did not choose a target path");
            return null;
        }

        private static Workspace GetWorkSpace(VersionControlServer vcs, IEnumerable<PendingChange> changes, ILogger logger, out string fromBranch, out List<string> toBranches)
        {
            IEnumerable<PendingChange> pendingEdits = changes.Where(p => p.IsEdit && !p.IsAdd && !p.IsDelete);
            if (pendingEdits.Any())
            {
                List<Workspace> workspaces = new List<Workspace>();
                workspaces.AddRange(vcs.QueryWorkspaces(null, vcs.AuthorizedUser, System.Net.Dns.GetHostName().Substring(0, 15)));
                workspaces.AddRange(vcs.QueryWorkspaces(null, vcs.AuthorizedUser, System.Net.Dns.GetHostName()));

                var workspace = GetBranches(vcs, workspaces, pendingEdits, logger, out fromBranch, out toBranches);
                if (workspace != null) { return workspace; }

                var targetWorkspace = AskUserForWorkspace(workspaces.ToList());
                if (targetWorkspace == null) { return targetWorkspace; }

                return GetBranches(vcs, workspaces, pendingEdits, logger, out fromBranch, out toBranches);
            }

            fromBranch = null;
            toBranches = new List<string>();
            return null;
        }

        private static Workspace GetBranches(VersionControlServer vcs, List<Workspace> workspaces, IEnumerable<PendingChange> pendingEdits, ILogger logger, out string fromBranch, out List<string> toBranches)
        {
            fromBranch = null;
            toBranches = new List<string>();
            foreach (var workspace in workspaces)
            {
                foreach (var pendingEdit in pendingEdits)
                {
                    string localItem = null;
                    try
                    {
                        localItem = workspace.GetLocalItemForServerItem(pendingEdit.ServerItem);
                    }
                    catch (ItemNotMappedException itemNotMappedException)
                    {
                        logger.Debug("Get local item for server item exception: " + itemNotMappedException.Message);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(localItem))
                    {
                        var branches = new List<string>();
                        foreach (ItemIdentifier mergeItem in vcs.QueryMergeRelationships(pendingEdit.ServerItem))
                        {
                            if (!mergeItem.IsDeleted && !string.IsNullOrWhiteSpace(mergeItem.Item))
                            {
                                branches.Add(mergeItem.Item);
                            }
                        }
                        if (branches.Any())
                        {
                            fromBranch = null;
                            toBranches.Clear();
                            foreach (var branchpath in branches)
                            {
                                string[] branchTree = branchpath.Split('/');
                                string[] serverTree = pendingEdit.ServerItem.Split('/');

                                int count = Math.Min(branchTree.Length, serverTree.Length);
                                for (int i = 1; i < count + 1; i++)
                                {
                                    if (branchTree[branchTree.Length - i] != serverTree[serverTree.Length - i])
                                    {
                                        toBranches.Add(string.Join("/", branchTree.Take(branchTree.Length - i + 1)));
                                        if (fromBranch == null)
                                            fromBranch = string.Join("/", serverTree.Take(serverTree.Length - i + 1));
                                        break;
                                    }
                                }
                            }
                            if (toBranches.Any())
                            {
                                toBranches.Add(fromBranch);
                                return workspace;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal enum UnshelveConflictResolution
        {
            KeepLocal,
            TakeShelved,
            Automerge,
            Manualmerge,
        }
        internal class MigrationRule
        {
            public string Source;
            public string Target;

            public MigrationRule(string source, string target)
            {
                this.Source = source;
                this.Target = target;
            }
        }

        internal class ShelvedItem
        {
            public bool ServerItemExists = true;
            public string ServerItem;
            public PendingChange LocalChange;
            public PendingChange ShelvedChange;
            public bool SameBranch;
            public string KeepLocalText;
            public string TakeShelvedText;
            public string MergeText;
            public UnshelveConflictState UnshelveConflictState;
            public string ShelvesetName;
            public string ShelvesetOwner;

            public ShelvedItem(PendingChange shelvedChange)
            {
                this.ShelvedChange = shelvedChange;
                this.ServerItem = shelvedChange.ServerItem;
            }
        }

        internal enum UnshelveConflictState
        {
            Unresolved,
            KeepLocal,
            TakeShelved,
            MergeSuccessful,
            MergeUnsuccessful,
        }

    }
}
