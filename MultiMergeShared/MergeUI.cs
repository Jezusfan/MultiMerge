using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using MultiMergeShared;

namespace MultiMerge
{
    public partial class MergeUI : Form
    {

        private DateTime? _mergeStart { get; set; }
        /// <summary>
        /// The version control server which will be used to find history
        /// </summary>
        private VersionControlServer _versionControlServer = null;

        private Workspace _workspace;

        private MergeWorker _processor;
        /// <summary>
        /// The version control ext
        /// </summary>
        private VersionControlExt versionControlExt = null;

        private ITeamExplorer _teamExplorer;
        private readonly ILogger _logger;

        private MergeUI()
        {
            this.InitializeComponent();

            FilesList.ListViewItemSorter = new ColumnSorter();
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeUI"/> class.
        /// </summary>
        public MergeUI(ILogger logger)
        {
            _logger = logger;
            _logger.Debug($"Constructing new instance of { nameof(MergeUI)}");
            this.InitializeComponent();

            FilesList.ListViewItemSorter = new ColumnSorter();
        }
#if DEBUG
        /// <summary>
        /// For stand-alone test purposes
        /// </summary>
        /// <param name="versionControlServer"></param>
        /// <param name="workspace"></param>
        /// <param name="path"></param>
        public MergeUI(VersionControlServer versionControlServer, Workspace workspace, ILogger logger, string path) : this(logger)
        {
            _versionControlServer = versionControlServer;
            _workspace = workspace;
            this.FromDatePicker.Value = DateTime.Now - new TimeSpan(28, 0, 0, 0);
            this.ToDatePicker.Value = DateTime.Now;
            var serverItem = _workspace.GetServerItemForLocalItem(path);
            txtBasePath.Text = serverItem;
            lblStatus.Text = string.Empty;
        }



        /// <summary>
        /// For stand-alone test purposes
        /// </summary>
        /// <param name="versionControlServer"></param>
        /// <param name="workspace"></param>
        /// <param name="path"></param>
        public MergeUI(VersionControlServer versionControlServer, Workspace workspace, ILogger logger, string path, int changeset) : this(versionControlServer, workspace, logger, path)
        {
            _versionControlServer = versionControlServer;
            _workspace = workspace;
            OptionsGroupBox.Visible = false;
            var criteria = CreateSearchCriteria();
            AnalyzeChangesetBackgroundWorker.RunWorkerAsync(new Func<BackgroundWorker, SearchCriteria>(bw => FindChangeSetsById(bw, new List<int>() { changeset }, criteria)));
        }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeUI"/> class, via Source Control Explorer.
        /// </summary>
        public MergeUI(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger) : this(logger)
        {
            this._teamExplorer = teamExplorer;
            this.versionControlExt = versionControl;
            this._versionControlServer = this.versionControlExt.Explorer.Workspace.VersionControlServer;
            _workspace = this.versionControlExt.Explorer.Workspace;
            this.txtBasePath.Text = this.versionControlExt.Explorer.CurrentFolderItem.SourceServerPath;
            this.FromDatePicker.Value = DateTime.Now - new TimeSpan(28, 0, 0, 0);
            this.ToDatePicker.Value = DateTime.Now;
            lblStatus.Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeUI"/> class, via History
        /// </summary>
        public MergeUI(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger,
            string path, List<int> changeSetids) : this(versionControl, teamExplorer, logger)
        {
            txtBasePath.Text = path;
            OptionsGroupBox.Visible = false;
            var criteria = CreateSearchCriteria();
            AnalyzeChangesetBackgroundWorker.RunWorkerAsync(new Func<BackgroundWorker, SearchCriteria>(bw => FindChangeSetsById(bw, changeSetids, criteria)));
        }

        #endregion

        #region Find Changesets

        /// <summary>
        /// Handles the Click event of the FindButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            // Check that we have a folder to search
            if (string.IsNullOrWhiteSpace(this.txtBasePath.Text))
            {
                MessageBox.Show("Please specify a source control folder to search", "Multi Merge", MessageBoxIcon.Exclamation);
                return;
            }

            // Check that we have a valid regular expression
            if (this.useRegularExpressions.Checked && !string.IsNullOrWhiteSpace(this.ContainingCommentText.Text))
            {
                try
                {
                    Regex rx = new Regex(this.ContainingCommentText.Text);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Please specify a valid regular expression", "Multi Merge", MessageBoxIcon.Exclamation);
                    return;
                }
            }

            var searchCriteria = CreateSearchCriteria();
            this.AnalyzeChangesetBackgroundWorker.RunWorkerAsync(new Func<BackgroundWorker, SearchCriteria>(bw => FindChangesetsByComment(bw, searchCriteria)));
        }

        private SearchCriteria CreateSearchCriteria()
        {
            var searchCriteria = new SearchCriteria();
            searchCriteria.SearchFile = this.txtBasePath.Text.Trim();
            searchCriteria.SearchUser = this.ByUserText.Text;
            searchCriteria.FromDateVersion = this.FromDatePicker.Checked
                ? new DateVersionSpec(this.FromDatePicker.Value)
                : null;
            searchCriteria.ToDateVersion = this.ToDatePicker.Checked
                ? new DateVersionSpec(this.ToDatePicker.Value)
                : null;
            searchCriteria.SearchComment = this.ContainingCommentText.Text;
            searchCriteria.UseRegex = this.useRegularExpressions.Checked;
            searchCriteria.KeepPreviousChangeSets = this.chkKeepChangesets.Checked;
            searchCriteria.IncludeRelevantChangeSets = this.chkIncludeRelevantChangesets.Checked;

            if (!searchCriteria.KeepPreviousChangeSets)
            {
                this.ResultsList.Items.Clear();
                this.FilesList.Items.Clear();
                lblStatus.Text = string.Empty;
            }

            this.FindButton.Enabled = false;
            this.MergeButton.Enabled = false;
            CancelFindButton.Enabled = true;
            return searchCriteria;
        }

        /// <summary>
        /// Handles the DoWork event of the FindChangesetsWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void AnalyzeChangesetBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;

            // Start looking for changesets
            Func<BackgroundWorker, SearchCriteria> actualWork = e.Argument as Func<BackgroundWorker, SearchCriteria>;
            e.Result = actualWork(bw);

            // If the operation was canceled by the user, 
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the FindChangesetsWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void FindChangesetsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // The user canceled the operation.
                MessageBox.Show("Operation was canceled", "Multi Merge", MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                MessageBox.Show(msg);
            }
            else
            {
                //the basePath may be changed after analyzing changeset ids
                var criteria = e.Result as SearchCriteria;
                if (criteria != null && criteria.SearchFile != txtBasePath.Text)
                    txtBasePath.Text = criteria.SearchFile;
                // The operation completed normally.
                // Check for zero things found
                if (this.ResultsList.Items.Count == 0)
                {
                    MessageBox.Show("No changesets found matching the search criteria", "Multi Merge", MessageBoxIcon.Information);
                }
            }
            lblStatus.Text = string.Format("{0} changed files found.", FilesList.Items.Count);
            MergeButton.Enabled = ResultsList.Items.Count > 0;
            FindButton.Enabled = true;
            CancelFindButton.Enabled = false;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the FindChangesetsWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void FindChangesetsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Changeset)
            {
                Changeset changeset = e.UserState as Changeset;
                _logger.Debug($"Changeset analyzed: {changeset.ChangesetId}");
                this.ResultsList.Items.Add(new ListViewItem(new string[] { changeset.ChangesetId.ToString(), changeset.Owner, changeset.CreationDate.ToString(), changeset.Changes.Length.ToString() ,changeset.Comment }));
            }
            else if (e.UserState != null && e.UserState.GetType() == typeof(Dictionary<string, List<Changeset>>))
            {
                var list = e.UserState as Dictionary<string, List<Changeset>>;
                this.FilesList.BeginUpdate();
                foreach (var pair in list)
                {
                    var item = FilesList.FindItemWithText(pair.Key);
                    string tooltip = string.Empty;
                    if (pair.Value?.Any()??false)
                        tooltip = $"Changeset ids missing:\n{ string.Join("\n",pair.Value.Select(c => $"{c.ChangesetId.ToString()} - {c.Comment}"))}";
                    item.ToolTipText = tooltip;
                    item.ImageIndex = string.IsNullOrEmpty(tooltip) ? 0 : 1;
                }
                this.FilesList.EndUpdate();
                _logger.Debug($"Files status updated for { list.Count} files.");
            }
            else if (e.UserState != null && e.UserState.GetType() == typeof(List<string>))
            {
                List<string> files = (e.UserState as List<string>).OrderBy(n => n).ToList();
                this.FilesList.BeginUpdate();
                this.FilesList.Items.AddRange(files.Select(f => new ListViewItem(f, 0){Checked = true}).ToArray());
                this.FilesList.EndUpdate();
                _logger.Debug($"File list updated with {files.Count} new files");
            }
            else if (e.UserState != null && e.UserState.GetType() == typeof(Action))
            {
                (e.UserState as Action).Invoke();
            }
        }
        
        private void MergeBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (e.UserState != null)
            {
                if (e.UserState is MergeWorker.MergeParameters)
                {
                    var config = e.UserState as MergeWorker.MergeParameters;
                    var question = new []{"There are more than 100 files which need analyzing if their target paths exists. ",
                                   "You can skip this if you know the target branch contains all files (i.e. update only).",
                                   "",
                                   "Click 'Yes' to skip, or 'No' to continue analysis"};

                    if (MessageBox.Show(question, "Multi Merge", MessageBoxButtons.YesNo, false) != DialogResult.No)
                        config.CancelAnalysis = true;
                }
                else if (e.UserState is Action)
                {
                    (e.UserState as Action).Invoke();
                }
                else
                {
                    _logger.Debug(e.UserState.ToString());
                    lblStatus.Text = string.Format($"{e.UserState} { (e.ProgressPercentage < 100? DateTime.Now.Subtract(_mergeStart??DateTime.Now).ToString(@"hh\:mm\:ss") : "")}");
                }
            }
        }

        private void MergeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FindButton.Enabled = true;
            MergeButton.Enabled = true;
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            CancelFindButton.Enabled = false;
            var duration = $" Merge took: { DateTime.Now.Subtract(_mergeStart ?? DateTime.Now).ToString(@"hh\:mm\:ss")} seconds";
            if (e.Error != null)
            {
                lblStatus.Text = string.Format(@"Error occurred during merge. {0}", duration);
                _logger.Error("Merge failed!", e.Error);
                MessageBox.Show("Bummer, the merge failed! See output window for more details.");
            }
            else
            {
                //by default we assume a successful merge, so we just append the duration
                lblStatus.Text += duration;
                if (e.Result != null)
                {
                    var errors = e.Result as List<string>;
                    if (errors.Any())
                    {
                        lblStatus.Text = $@"{errors.Count} error{(errors.Count==1?"":"s")} occurred during merge! {duration} ";
                        errors.ForEach(error => _logger.Error(error));
                        MessageBox.Show($"{errors.Count} errors occurred during merge. Check output window for more details", "Multi Merge", MessageBoxIcon.Error);
                    }
                }
                
            }
        }

        private void MergeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;
            var mergeConfig = e.Argument as MergeWorker.MergeParameters;

            var cancelAnalysis = false;

            if (_processor.FileCount > 100 && _processor.ChangesetCount > 1)
                bw.ReportProgress(0, mergeConfig);

            e.Result = _processor.MergeChanges(mergeConfig, bw);
        }



        /// <summary>
        /// Finds the changesets by comment.
        /// </summary>
        /// <param name="bw">The background worker.</param>
        /// <param name="sc">The search criteria.</param>
        /// <returns>An error code</returns>
        private SearchCriteria FindChangesetsByComment(BackgroundWorker bw, SearchCriteria sc)
        {
            try
            {
                MergeWorker processor = GetProcessor(!sc.KeepPreviousChangeSets, bw);
                IEnumerable changesets = this._versionControlServer.QueryHistory(
                    sc.SearchFile,
                    VersionSpec.Latest,
                    0,
                    RecursionType.Full,
                    string.IsNullOrWhiteSpace(sc.SearchUser) ? null : sc.SearchUser,
                    sc.FromDateVersion,
                    sc.ToDateVersion,
                    Int32.MaxValue,
                    true,
                    false);
                foreach (Changeset changeset in changesets)
                {
                    if (bw.CancellationPending)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(sc.SearchComment) ||
                        (!sc.UseRegex && changeset.Comment.IndexOf(sc.SearchComment.Trim(), StringComparison.CurrentCultureIgnoreCase) != -1) ||
                        (sc.UseRegex && Regex.IsMatch(changeset.Comment, sc.SearchComment)))
                    {
                        processor.AnalyseChangeSet(changeset);
                    }
                }

                processor.DetectMissingChangeSets(sc.IncludeRelevantChangeSets);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Multi Merge", MessageBoxIcon.Stop);
            }
            return sc;
        }

        private SearchCriteria FindChangeSetsById(BackgroundWorker bw, List<int> ids, SearchCriteria criteria)
        {
            try
            {
                _logger.Debug($"Finding ChangeSets By Id started, using ids: { string.Join(", ", ids)}");
                MergeWorker processor = GetProcessor(!criteria.KeepPreviousChangeSets, bw);
                //change our 'searchCriteria.FilePath' to match all files found in a changeSet
                processor.FilesAdded += (filenames) => criteria.SearchFile = GetMatchingStart(criteria.SearchFile, filenames).TrimEnd('\\', '/');
                var changesets = ids.Select(id =>
                {
                    _logger.Debug($"VersionControlServer.GetChangeset({id}, true, false)");
                    return _versionControlServer.GetChangeset(id, true, false);
                });
                _logger.Debug($"Analyzing changesets...");
                foreach (Changeset changeset in changesets)
                {

                    if (bw.CancellationPending)
                    {
                        break;
                    }
                    _logger.Debug($"Analyzing changeset: {changeset.ChangesetId }");
                    processor.AnalyseChangeSet(changeset);

                }

                if (!bw.CancellationPending)
                {
                    processor.DetectMissingChangeSets(chkIncludeRelevantChangesets.Checked);
                    _logger.Debug("Finding ChangeSets By Id completed");
                }
                else
                {
                    _logger.Debug("Finding ChangeSets By Id was cancelled by user");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Multi Merge", MessageBoxIcon.Stop);
            }
            return criteria;
        }

        private string GetMatchingStart(string first, List<string> files)
        {
            foreach (var second in files)
            {
                int length = first.Zip(second, (c1, c2) => c1 == c2).TakeWhile(b => b).Count();
                var old = first;
                if (length != first.Length)
                {
                    first = first.Substring(0, length);
                    _logger.Debug($"File in changeset requires path change, from: {old} to: {first}");
                }
            }
            return first;
        }

        private MergeWorker GetProcessor(bool forceNewInstance, BackgroundWorker bw)
        {
            if (_processor == null || forceNewInstance)
            {
                _processor = new MergeWorker(_workspace, _versionControlServer, _teamExplorer, _logger);
                _processor.ChangeSetAdded += c => bw.ReportProgress(0, c);
                _processor.FilesAdded += f => bw.ReportProgress(0, f);
                _processor.MissingChangesForFileDetected += m => bw.ReportProgress(0, m);
                _processor.MergeProgressChanged += (percentage, status) => MergeBackgroundWorker.ReportProgress(percentage, status);
            }
            return _processor;
        }

        #endregion

        #region Merge Changesets

        private void MergeButton_Click(object sender, EventArgs eventArgs)
        {
            string basePath = txtBasePath.Text;
            bool baselessMerge;
            string targetpath = _processor.GetEligibleMergePath(basePath, out baselessMerge);
            if (!string.IsNullOrEmpty(targetpath))
            {
                FindButton.Enabled = false;
                MergeButton.Enabled = false;
                progressBar1.Visible = true;
                CancelFindButton.Enabled = true;
                _mergeStart = DateTime.Now;

                var config = new MergeWorker.MergeParameters()
                {
                    BaselessMerge = baselessMerge,
                    BasePath = basePath,
                    TargetPath = targetpath,
                    MergeToLatest = chkMergeAllChanges.Checked,
                    MergeIntersectingFiles = chkMergeIntersectingFiles.Checked
                };
                if (OptionsGroupBox.Visible)
                {
                    config.ExcludedFiles.AddRange(GetUnCheckedItems(FilesList.Items));
                }

                this.MergeBackgroundWorker.RunWorkerAsync(config);
            }
        }

        private IEnumerable<string> GetUnCheckedItems(ListView.ListViewItemCollection filesListItems)
        {
            foreach (ListViewItem filesListItem in filesListItems)
            {
                if (!filesListItem.Checked)
                    yield return filesListItem.Text;
            }
        }

        #endregion

        #region Other control events

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            CancelFindButton_Click(this, null);
            this.Close();
        }

        /// <summary>
        /// Handles the DoubleClick event of the ResultsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ResultsList_DoubleClick(object sender, EventArgs e)
        {
            if (this.ResultsList.SelectedItems.Count == 0)
            {
                return;
            }

            int changesetId;
            if (int.TryParse(this.ResultsList.SelectedItems[0].Text, out changesetId))
                this.versionControlExt.ViewChangesetDetails(changesetId);

        }
        
        private void FilesList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.Selected = true;
            FilesList.Select();
        }

        private void FilesList_DoubleClick(object sender, EventArgs e)
        {
            if (this.FilesList.SelectedItems.Count == 0)
            {
                return;
            }
            FilesList.SelectedItems[0].Checked = !FilesList.SelectedItems[0].Checked;
            var file = FilesList.SelectedItems[0].Text;
            this.versionControlExt?.History?.Show(file, VersionSpec.Latest, 0, RecursionType.Full);
        }

        /// <summary>
        /// Handles the Click event of the CopyButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopyButton_Click(object sender, EventArgs e)
        {
            StringBuilder clipboardText = new StringBuilder();
            if (this.ChangesetsAndFiles.SelectedTab == this.ChangesetList)
            {
                var fileCounts = _processor?.FileCountPerChangeSet()??new Dictionary<string, int>();
                // Copy list of changesets
                foreach (ListViewItem changeset in this.ResultsList.Items)
                {
                    
                    clipboardText.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", changeset.SubItems[0].Text, changeset.SubItems[1].Text,
                        changeset.SubItems[2].Text, changeset.SubItems[4].Text, fileCounts[changeset.SubItems[0].Text]));
                }
            }
            else
            {
                // Copy list of _files
                foreach (ListViewItem listItem in this.FilesList.Items)
                {
                    clipboardText.AppendLine(listItem.Text);
                }
            }

            if (clipboardText.Length > 0)
            {
                Clipboard.SetText(clipboardText.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CancelFindButton_Click(object sender, EventArgs e)
        {
            if (this.AnalyzeChangesetBackgroundWorker.IsBusy)
            {
                this.AnalyzeChangesetBackgroundWorker.CancelAsync();
            }
            if (this.MergeBackgroundWorker.IsBusy)
            {
                this.MergeBackgroundWorker.CancelAsync();
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the ResultsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void ResultsList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                this.ResultsList_DoubleClick(sender, null);
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyButton_Click(null, null);
            }
        }

        #endregion

    }
}
