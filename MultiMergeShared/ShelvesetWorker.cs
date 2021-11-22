using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using MultiMerge;

namespace MultiMergeShared
{
    internal class ShelvesetWorker
    {


        public string UnshelveMerge(string shelvesetName, string shelvesetOwner, PendingChange[] pendingChanges, MergeWorker.MigrationRule migrationRule, Workspace workspace, ILogger logger, UpdateLocalVersionQueue queue)
        {

            List<ItemSpec> itemSpecList = new List<ItemSpec>();
            Dictionary<int, PendingChange> dictionary1 = new Dictionary<int, PendingChange>();
            Dictionary<string, PendingChange> dictionary2 = new Dictionary<string, PendingChange>();
            foreach (PendingChange workspacePendingChange in workspace.GetPendingChanges())
            {
                if (workspacePendingChange.ItemId != 0)
                    dictionary1[workspacePendingChange.ItemId] = workspacePendingChange;
                dictionary2[workspacePendingChange.ServerItem] = workspacePendingChange;
            }
            List<MergeWorker.ShelvedItem> shelvedItemList1 = new List<MergeWorker.ShelvedItem>(pendingChanges.Length);
            List<MergeWorker.ShelvedItem> shelvedItemList2 = new List<MergeWorker.ShelvedItem>();
            foreach (PendingChange pendingChange in pendingChanges)
                shelvedItemList1.Add(new MergeWorker.ShelvedItem(pendingChange));
            using (List<MergeWorker.ShelvedItem>.Enumerator enumerator = shelvedItemList1.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    MergeWorker.ShelvedItem current = enumerator.Current;
                    current.ShelvesetName = shelvesetName;
                    current.ShelvesetOwner = shelvesetOwner;

                    if ((current.ServerItem).ToLowerInvariant().Contains(migrationRule.Source.ToLowerInvariant()))
                    {
                        current.SameBranch = true;
                        current.ServerItem = (migrationRule.Target + (current.ServerItem).Substring((migrationRule.Source).Length));
                    }

                    if (!workspace.IsServerPathMapped(current.ServerItem))
                        throw new Exception($"The target branch {migrationRule.Target} is out of date. Do a GetLatest and try again.");
                    if (!current.SameBranch)
                    {
                        if (dictionary1.ContainsKey((current.ShelvedChange).ItemId))
                        {
                            current.LocalChange = dictionary1[current.ShelvedChange.ItemId];
                            current.KeepLocalText = "KeepMyLocalChanges";
                            current.TakeShelvedText = "UndoAndTakeShelved";
                            current.MergeText = "KeepMyLocalChangesAndMerge";
                            shelvedItemList2.Add(current);
                        }
                        else if (dictionary2.ContainsKey(current.ShelvedChange.ServerItem))
                        {
                            current.LocalChange = dictionary2[current.ShelvedChange.ServerItem];
                            if (current.LocalChange.IsAdd || current.LocalChange.IsBranch || (current.LocalChange.IsRename || current.LocalChange.IsUndelete))
                            {
                                current.KeepLocalText = "KeepMyLocalChanges";
                                current.TakeShelvedText = "UndoAndTakeShelved";
                                current.MergeText = "KeepMyLocalChangesAndMerge";
                                shelvedItemList2.Add(current);
                            }
                            else
                            {
                                current.SameBranch = true;
                                logger.Debug($"The item ids do not match for {current.ServerItem} and so it will be treated as a migrated change.");
                                current.KeepLocalText = "KeepMyLocalChanges";
                                current.TakeShelvedText = "UndoAndMigrate";
                                current.MergeText = "KeepMyLocalChangesAndMerge";
                                shelvedItemList2.Add(current);
                            }
                        }
                        else
                            itemSpecList.Add(ItemSpec.FromPendingChanges(new PendingChange[1] { current.ShelvedChange })[0]);
                    }
                    else
                    {
                        if (dictionary2.ContainsKey(current.ServerItem))
                        {
                            current.LocalChange = dictionary2[current.ServerItem];
                            current.KeepLocalText = "KeepMyLocalChanges";
                            current.TakeShelvedText = "UndoAndMigrate";
                            if (!current.LocalChange.IsDelete)
                                current.MergeText = "KeepMyLocalChangesAndMerge";
                        }
                        else
                        {
                            current.KeepLocalText = "SkipThisShelvedChange";
                            current.TakeShelvedText = "MigrateShelvedContent";
                            current.ServerItemExists = workspace.VersionControlServer.ServerItemExists(current.ServerItem, current.ShelvedChange.ItemType);
                            if (current.ServerItemExists)
                                current.MergeText = "MergeLocalAndShelved";
                        }
                        shelvedItemList2.Add(current);
                    }
                }
            }
            var successful = itemSpecList.Count + shelvedItemList2.Count;
            var message = $"{successful} items were unshelved successfully";
            if (migrationRule.Source != migrationRule.Target)
                message += $" to target branch: {migrationRule.Target}";
            if (itemSpecList.Count > 0)
            {
                workspace.Unshelve(shelvesetName, shelvesetOwner, itemSpecList.ToArray());
            }
            if (shelvedItemList2.Count > 0)
            {
                foreach (var shelvedItem in shelvedItemList2)
                {
                    shelvedItem.UnshelveConflictState = PendResolvedUnshelveConflict(shelvedItem, MergeWorker.UnshelveConflictResolution.Automerge, workspace, logger, queue);
                }

                var failedMerges = shelvedItemList2.Where(s => s.UnshelveConflictState == MergeWorker.UnshelveConflictState.MergeUnsuccessful).ToList();
                if (failedMerges.Any())
                {
                    logger.Debug("Solving conflicts by taking Shelved files...");
                    foreach (var failedMerge in failedMerges)
                    {
                        logger.Debug("Overwriting local with shelved file:");
                        PendResolvedUnshelveConflict(failedMerge, MergeWorker.UnshelveConflictResolution.TakeShelved, workspace, logger, queue);
                    }
                    message = $"{message}, but there were {failedMerges.Count} conflicts resolved by taking the shelved file. Check the output window for more details.";
                }
                logger.Debug(message);
                //do not show UI to resolve conflicts for now...
                //using (DialogUnshelveResolve dialogUnshelveResolve = new DialogUnshelveResolve(shelvedItemList2))
                //{
                //    dialogUnshelveResolve.PendResolvedUnshelveConflict += (i, r) => PendResolvedUnshelveConflict(i, r, workspace, logger, queue);

                //    if (((BaseDialog)dialogUnshelveResolve).ShowDialog() == DialogResult.Abort)
                //        throw new Exception("User aborted merge");
                //}
            }
            return message;
        }

        internal MergeWorker.UnshelveConflictState PendResolvedUnshelveConflict(MergeWorker.ShelvedItem item, MergeWorker.UnshelveConflictResolution resolution, Workspace workspace, ILogger logger, UpdateLocalVersionQueue queue)
        {
            logger.Debug(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "\n{0}: ", (object)item.ServerItem));
            if (!item.ServerItemExists && (resolution == MergeWorker.UnshelveConflictResolution.Automerge || resolution == MergeWorker.UnshelveConflictResolution.Manualmerge))
                resolution = MergeWorker.UnshelveConflictResolution.TakeShelved;
            switch (resolution)
            {
                case MergeWorker.UnshelveConflictResolution.KeepLocal:
                    logger.Debug("PreservingLocalChange");
                    item.UnshelveConflictState = MergeWorker.UnshelveConflictState.KeepLocal;
                    break;
                case MergeWorker.UnshelveConflictResolution.TakeShelved:

                    item.UnshelveConflictState = MergeWorker.UnshelveConflictState.TakeShelved;
                    if (item.LocalChange != null)
                    {
                        if (workspace.Undo(new PendingChange[1] { item.LocalChange }) == 0)
                            logger.Error("UnableToUndoChanges");
                    }
                    if (!item.SameBranch)
                    {
                        logger.Debug("UnshelvingChange");
                        try
                        {
                            workspace.Unshelve(item.ShelvesetName, item.ShelvesetOwner, ItemSpec.FromPendingChanges(new PendingChange[1]
                            {
                item.ShelvedChange
                            }));
                            break;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message, ex);
                            item.UnshelveConflictState = MergeWorker.UnshelveConflictState.Unresolved;
                            throw;
                        }
                    }
                    else
                    {
                        bool flag = workspace.VersionControlServer.ServerItemExists(item.ServerItem, VersionSpec.Latest, DeletedState.NonDeleted, item.ShelvedChange.ItemType);
                        if (item.ShelvedChange.IsDelete && !flag)
                        {
                            logger.Debug($"SkippingDeleteOnNonexistentItem { item.ServerItem}");
                            break;
                        }
                        if (item.ShelvedChange.IsUndelete && !flag)
                        {
                            ExtendedItem[] extendedItems = workspace.VersionControlServer.GetExtendedItems(item.ServerItem, DeletedState.Deleted, item.ShelvedChange.ItemType);
                            if (extendedItems.Length != 0)
                            {
                                if (workspace.PendUndelete(item.ServerItem, extendedItems[0].DeletionId) == 0)
                                    logger.Debug($"CannotMigrateUndelete {item.ServerItem}");
                                else
                                    flag = true;
                            }
                        }
                        if (item.ShelvedChange.IsBranch && !flag)
                        {
                            GetStatus getStatus = workspace.Merge(item.ShelvedChange.SourceServerItem, item.ServerItem, (VersionSpec)null, VersionSpec.Latest, LockLevel.Unchanged, RecursionType.None, MergeOptionsEx.None);
                            if (getStatus == null || getStatus.NumConflicts > 0 || getStatus.NumFailures > 0)
                            {
                                workspace.Undo(item.ServerItem);
                                logger.Debug($"CannotMigrateBranch {item.ServerItem}");
                            }
                            else
                                flag = true;
                        }
                        if (!flag)
                        {
                            string itemForServerItem = workspace.TryGetLocalItemForServerItem(item.ServerItem);
                            if (item.ShelvedChange.ItemType == ItemType.File)
                            {
                                string tempFileName = FileSpec.GetTempFileName();
                                item.ShelvedChange.DownloadShelvedFile(tempFileName);
                                FileSpec.CopyFile(tempFileName, itemForServerItem, true);
                                workspace.PendAdd(itemForServerItem, false);
                                FileSpec.DeleteFileWithoutException(tempFileName);
                                break;
                            }
                            Directory.CreateDirectory(itemForServerItem);
                            workspace.PendAdd(itemForServerItem, false);
                            break;
                        }
                        if (item.ShelvedChange.IsDelete)
                        {
                            workspace.PendDelete(item.ServerItem);
                            break;
                        }
                        if (item.ShelvedChange.ItemType == ItemType.File && (item.ShelvedChange.IsEdit || !item.ShelvedChange.IsBranch))
                        {
                            workspace.PendEdit(item.ServerItem);
                            string itemForServerItem = workspace.TryGetLocalItemForServerItem(item.ServerItem);
                            string tempFileName = FileSpec.GetTempFileName();
                            item.ShelvedChange.DownloadShelvedFile(tempFileName);
                            FileSpec.CopyFile(tempFileName, itemForServerItem, true);
                            FileSpec.DeleteFileWithoutException(tempFileName);
                            break;
                        }
                        break;
                    }
                case MergeWorker.UnshelveConflictResolution.Automerge:
                    logger.Debug("StartingAutoMerge");
                    if (item.ShelvedChange.ItemType == ItemType.Folder && !item.ShelvedChange.IsDelete)
                    {
                        item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeSuccessful;
                        break;
                    }
                    if (item.ShelvedChange.IsDelete)
                    {
                        if (item.LocalChange != null || !item.ServerItemExists)
                        {
                            item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeUnsuccessful;
                            break;
                        }
                        workspace.PendDelete(item.ServerItem);
                        item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeSuccessful;
                        break;
                    }
                    ThreeWayMerge threeWayMerge1 = this.PreMerge(item, workspace);
                    threeWayMerge1.UseExternalMergeTool = false;
                    string localItem1 = item.LocalChange.LocalItem;
                    if (threeWayMerge1.OriginalFileEncoding != null && threeWayMerge1.ModifiedFileEncoding != null && (threeWayMerge1.LatestFileEncoding != null && threeWayMerge1.MergedFileEncoding != null))
                    {
                        threeWayMerge1.Run();
                        MergeSummary contentMergeSummary = threeWayMerge1.ContentMergeSummary;
                        if (contentMergeSummary.TotalConflicting > 0)
                        {
                            logger.Debug($"AutoMergeLeftConflicts {contentMergeSummary.TotalConflicting}");
                            item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeUnsuccessful;
                        }
                        else
                        {

                            File.Copy(threeWayMerge1.MergedFileName, localItem1, true);
                            if (item.LocalChange != null && !item.SameBranch && (item.LocalChange.ItemId == item.ShelvedChange.ItemId && item.ShelvedChange.Version > item.LocalChange.Version))
                                queue.QueueUpdate(item.ShelvedChange.SourceServerItem, item.LocalChange.ItemId, item.LocalChange.LocalItem, item.ShelvedChange.Version);
                            logger.Debug("AutoMergeSuccessful");
                            item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeSuccessful;
                        }
                        FileSpec.DeleteFileWithoutException(threeWayMerge1.MergedFileName);
                    }
                    else
                    {
                        logger.Debug($"AutoMergeFailedBadEncoding {item.ShelvedChange.ServerItem}");
                        item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeUnsuccessful;
                    }
                    FileSpec.DeleteFileWithoutException(threeWayMerge1.OriginalFileName);
                    if (FileSpec.Equals(item.LocalChange.LocalItem, threeWayMerge1.LatestFileName))
                    {
                        FileSpec.DeleteFileWithoutException(threeWayMerge1.ModifiedFileName);
                        break;
                    }
                    FileSpec.DeleteFileWithoutException(threeWayMerge1.LatestFileName);
                    break;
                case MergeWorker.UnshelveConflictResolution.Manualmerge:
                    ThreeWayMerge threeWayMerge2 = this.PreMerge(item, workspace);
                    threeWayMerge2.UseExternalMergeTool = true;
                    string localItem2 = item.LocalChange.LocalItem;
                    if (!threeWayMerge2.Run())
                    {
                        logger.Debug("ThirdPartyMergeUnsuccessful");
                        item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeUnsuccessful;
                    }
                    else
                    {

                        File.Copy(threeWayMerge2.MergedFileName, localItem2, true);
                        if (item.LocalChange != null && !item.SameBranch && (item.LocalChange.ItemId == item.ShelvedChange.ItemId && item.ShelvedChange.Version > item.LocalChange.Version))
                            queue.QueueUpdate(item.ShelvedChange.SourceServerItem, item.LocalChange.ItemId, item.LocalChange.LocalItem, item.ShelvedChange.Version);
                        logger.Debug("ThirdPartyMergeSuccessful");
                        item.UnshelveConflictState = MergeWorker.UnshelveConflictState.MergeSuccessful;
                    }
                    FileSpec.DeleteFileWithoutException(threeWayMerge2.MergedFileName);
                    FileSpec.DeleteFileWithoutException(threeWayMerge2.OriginalFileName);
                    if (FileSpec.Equals(item.LocalChange.LocalItem, threeWayMerge2.LatestFileName))
                    {
                        FileSpec.DeleteFileWithoutException(threeWayMerge2.ModifiedFileName);
                        break;
                    }
                    FileSpec.DeleteFileWithoutException(threeWayMerge2.LatestFileName);
                    break;
            }
            return item.UnshelveConflictState;
        }

        private ThreeWayMerge PreMerge(MergeWorker.ShelvedItem item, Workspace workspace)
        {
            ThreeWayMerge threeWayMerge1 = new ThreeWayMerge();
            if (!item.SameBranch && item.LocalChange != null && (item.LocalChange.ItemId == item.ShelvedChange.ItemId && item.LocalChange.Version > 0))
            {
                threeWayMerge1.OriginalFileName = FileSpec.GetTempFileNameWithExtension(Path.GetExtension(item.LocalChange.LocalItem));
                if (item.LocalChange.Version <= item.ShelvedChange.Version)
                {
                    if (item.LocalChange.IsEncoding)
                    {
                        ItemSet items = workspace.VersionControlServer.GetItems(item.LocalChange.ServerItem, (VersionSpec)new ChangesetVersionSpec(item.LocalChange.Version), RecursionType.None);
                        threeWayMerge1.OriginalFileEncoding = TryGetEncoding(items.Items[0].Encoding);
                    }
                    else
                        threeWayMerge1.OriginalFileEncoding = TryGetEncoding(item.LocalChange.Encoding);
                    item.LocalChange.DownloadBaseFile(threeWayMerge1.OriginalFileName);
                    threeWayMerge1.ModifiedFileName = item.LocalChange.LocalItem;
                    threeWayMerge1.ModifiedFileEncoding = TryGetEncoding(item.LocalChange.Encoding);
                    ThreeWayMerge threeWayMerge2 = threeWayMerge1;
                    threeWayMerge2.MergedFileEncoding = threeWayMerge2.ModifiedFileEncoding;
                    threeWayMerge1.LatestFileName = FileSpec.GetTempFileNameWithExtension(Path.GetExtension(item.LocalChange.LocalItem));
                    threeWayMerge1.LatestFileEncoding = TryGetEncoding(item.ShelvedChange.Encoding);
                    item.ShelvedChange.DownloadShelvedFile(threeWayMerge1.LatestFileName);
                    threeWayMerge1.OriginalFileLabel = $"MergeLabelLocalNormal {item.LocalChange.Version}";
                    threeWayMerge1.ModifiedFileLabel = $"MergeLabelLocalPostVersion {item.LocalChange.Version}";
                    threeWayMerge1.LatestFileLabel = $"MergeLabelShelvedPostVersion {item.ShelvedChange.Version}";
                }
                else
                {
                    if (item.ShelvedChange.IsEncoding)
                    {
                        ItemSet items = workspace.VersionControlServer.GetItems(item.ShelvedChange.ServerItem, (VersionSpec)new ChangesetVersionSpec(item.ShelvedChange.Version), RecursionType.None);
                        threeWayMerge1.OriginalFileEncoding = TryGetEncoding(items.Items[0].Encoding);
                    }
                    else
                        threeWayMerge1.OriginalFileEncoding = TryGetEncoding(item.ShelvedChange.Encoding);
                    item.ShelvedChange.DownloadBaseFile(threeWayMerge1.OriginalFileName);
                    threeWayMerge1.ModifiedFileName = FileSpec.GetTempFileNameWithExtension(Path.GetExtension(item.LocalChange.LocalItem));
                    threeWayMerge1.ModifiedFileEncoding = TryGetEncoding(item.ShelvedChange.Encoding);
                    item.ShelvedChange.DownloadShelvedFile(threeWayMerge1.ModifiedFileName);
                    threeWayMerge1.LatestFileName = item.LocalChange.LocalItem;
                    threeWayMerge1.LatestFileEncoding = TryGetEncoding(item.LocalChange.Encoding);
                    ThreeWayMerge threeWayMerge2 = threeWayMerge1;
                    threeWayMerge2.MergedFileEncoding = threeWayMerge2.LatestFileEncoding;
                    threeWayMerge1.OriginalFileLabel = $"MergeLabelShelvedNormal {item.ShelvedChange.Version}";
                    threeWayMerge1.ModifiedFileLabel = $"MergeLabelShelvedPostVersion {item.ShelvedChange.Version}";
                    threeWayMerge1.LatestFileLabel = $"MergeLabelLocalPostVersion {item.LocalChange.Version}";
                }
            }
            else
            {
                if (item.LocalChange == null)
                {
                    workspace.PendEdit(item.ServerItem, RecursionType.None);

                    PendingChange[] pendingChanges = workspace.GetPendingChanges(item.ServerItem, RecursionType.None);
                    item.LocalChange = pendingChanges[0];
                }
                threeWayMerge1.OriginalFileName = FileSpec.GetTempFileName();
                if (item.ShelvedChange.IsAdd || item.ShelvedChange.IsBranch)
                    threeWayMerge1.IsBaseless = true;
                else
                    item.ShelvedChange.DownloadBaseFile(threeWayMerge1.OriginalFileName);
                threeWayMerge1.ModifiedFileName = item.LocalChange.LocalItem;
                threeWayMerge1.ModifiedFileEncoding = TryGetEncoding(item.LocalChange.Encoding);
                ThreeWayMerge threeWayMerge2 = threeWayMerge1;
                threeWayMerge2.MergedFileEncoding = threeWayMerge2.ModifiedFileEncoding;
                threeWayMerge1.LatestFileName = FileSpec.GetTempFileNameWithExtension(Path.GetExtension(item.LocalChange.LocalItem));
                threeWayMerge1.LatestFileEncoding = TryGetEncoding(item.ShelvedChange.Encoding);
                item.ShelvedChange.DownloadShelvedFile(threeWayMerge1.LatestFileName);
                ThreeWayMerge threeWayMerge3 = threeWayMerge1;
                threeWayMerge3.OriginalFileEncoding = threeWayMerge3.LatestFileEncoding;
                threeWayMerge1.OriginalFileLabel = "Generated base";
                threeWayMerge1.ModifiedFileLabel = $"MergeLabelLocalPostVersion {item.LocalChange.Version}";
                threeWayMerge1.LatestFileLabel = $"MergeLabelShelvedPostVersion {item.ShelvedChange.Version}";
            }
            return threeWayMerge1;
        }

        private static Encoding TryGetEncoding(int encodingAsInt)
        {
            if (encodingAsInt < 0 || encodingAsInt > (int)ushort.MaxValue)
                return (Encoding)null;
            return Encoding.GetEncoding(encodingAsInt);
        }
    }
}
