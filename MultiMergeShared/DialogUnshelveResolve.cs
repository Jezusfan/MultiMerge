// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PowerTools.CommandLine.CommandUnshelve
// Assembly: TFPT, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A843CA-B1F0-495E-8F2B-DAE1FEAC9C4F
// Assembly location: D:\Temp\powertools\TFPT.EXE

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using MultiMerge;
using MessageBox = MultiMerge.MessageBox;

namespace MultiMergeShared
{
    internal class DialogUnshelveResolve : BaseDialog
    {
        private Dictionary<ListViewItem, MergeWorker.ShelvedItem> m_conflicts = new Dictionary<ListViewItem, MergeWorker.ShelvedItem>();
        private Dictionary<MergeWorker.ShelvedItem, ListViewItem> m_conflictsReverseLookup = new Dictionary<MergeWorker.ShelvedItem, ListViewItem>();
        private ColumnHeader[] m_columnHeaders;
        private ColumnSorter m_columnSorter;
        private int m_unresolvedConflicts;
        public PendResolvedUnshelveConflictDelegate PendResolvedUnshelveConflict;
        private IContainer components;
        private Button buttonResolve;
        private ListView listViewFiles;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label labelFiles;
        private Label labelSummary;
        private Button buttonAll;
        private Button buttonClose;

        public DialogUnshelveResolve(List<MergeWorker.ShelvedItem> itemList)
        {
            this.InitializeComponent();
            this.m_columnSorter = new ColumnSorter();
            this.m_columnSorter.CurrentColumn = 0;
            this.CreateColumns();
            this.buttonAll.Enabled = false;
            this.buttonResolve.Enabled = false;
            this.listViewFiles.BeginUpdate();
            foreach (MergeWorker.ShelvedItem shelvedItem in itemList)
                this.AddUnshelveConflict(shelvedItem);
            this.listViewFiles.ListViewItemSorter = (IComparer)this.m_columnSorter;
            this.listViewFiles.Sort();
            this.listViewFiles.EndUpdate();
            this.labelSummary.Text = "InstructionsResolveConflicts";
        }

        private void AddUnshelveConflict(MergeWorker.ShelvedItem item)
        {
            ListViewItem key = new ListViewItem(new string[3]
            {
        VersionControlPath.GetFileName(item.ServerItem),
        VersionControlPath.GetFolderName(item.ServerItem),
        "UnresolvedConflict"
            });
            this.listViewFiles.Items.Add(key);
            this.m_conflicts.Add(key, item);
            this.m_conflictsReverseLookup.Add(item, key);
            ++this.m_unresolvedConflicts;
            this.UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (this.listViewFiles.SelectedItems.Count > 0 && (this.m_conflicts[this.listViewFiles.SelectedItems[0]].UnshelveConflictState == MergeWorker.UnshelveConflictState.Unresolved || this.m_conflicts[this.listViewFiles.SelectedItems[0]].UnshelveConflictState == MergeWorker.UnshelveConflictState.MergeUnsuccessful))
                this.buttonResolve.Enabled = true;
            else
                this.buttonResolve.Enabled = false;
            if (this.m_unresolvedConflicts == 0)
                this.labelSummary.Text = "InstructionsConflictsResolved";
            if (this.m_unresolvedConflicts > 0)
                this.buttonAll.Enabled = true;
            else
                this.buttonAll.Enabled = false;
        }

        private void ResolveUnshelveConflict(ListViewItem toResolve)
        {
            MergeWorker.ShelvedItem conflict = this.m_conflicts[toResolve];
            switch (new DialogUnshelveResolveChange(conflict).ShowDialog())
            {
                case DialogResult.Retry:
                    MergeWorker.UnshelveConflictState unshelveConflictState1 = this.PendResolvedUnshelveConflict(conflict, MergeWorker.UnshelveConflictResolution.Automerge);
                    if (MergeWorker.UnshelveConflictState.MergeSuccessful == unshelveConflictState1)
                    {
                        toResolve.SubItems[2].Text = "ChangesMerged";
                        --this.m_unresolvedConflicts;
                        break;
                    }
                    if (MergeWorker.UnshelveConflictState.MergeUnsuccessful == unshelveConflictState1)
                    {
                        toResolve.SubItems[2].Text = "MergeFailed";
                        break;
                    }
                    break;
                case DialogResult.Ignore:
                    MergeWorker.UnshelveConflictState unshelveConflictState2 = this.PendResolvedUnshelveConflict(conflict, MergeWorker.UnshelveConflictResolution.Manualmerge);
                    if (MergeWorker.UnshelveConflictState.MergeSuccessful == unshelveConflictState2)
                    {
                        toResolve.SubItems[2].Text = "ChangesMerged";
                        --this.m_unresolvedConflicts;
                        break;
                    }
                    if (MergeWorker.UnshelveConflictState.MergeUnsuccessful == unshelveConflictState2)
                    {
                        toResolve.SubItems[2].Text = "MergeFailed";
                        break;
                    }
                    break;
                case DialogResult.Yes:
                    if (MergeWorker.UnshelveConflictState.TakeShelved == this.PendResolvedUnshelveConflict(conflict, MergeWorker.UnshelveConflictResolution.TakeShelved))
                    {
                        toResolve.SubItems[2].Text = "ResolvedShelvedChange";
                        --this.m_unresolvedConflicts;
                        break;
                    }
                    break;
                case DialogResult.No:
                    if (MergeWorker.UnshelveConflictState.KeepLocal == this.PendResolvedUnshelveConflict(conflict, MergeWorker.UnshelveConflictResolution.KeepLocal))
                    {
                        toResolve.SubItems[2].Text = "ResolvedLocalChange";
                        --this.m_unresolvedConflicts;
                        break;
                    }
                    break;
            }
            this.UpdateButtonStates();
        }

        private void listViewFiles_ItemSelectionChanged(
          object sender,
          ListViewItemSelectionChangedEventArgs e)
        {
            this.UpdateButtonStates();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonResolve_Click(object sender, EventArgs e)
        {
            this.ResolveUnshelveConflict(this.listViewFiles.SelectedItems[0]);
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem key in this.m_conflicts.Keys)
            {
                MergeWorker.ShelvedItem conflict = this.m_conflicts[key];
                if (conflict.UnshelveConflictState == MergeWorker.UnshelveConflictState.Unresolved || conflict.UnshelveConflictState == MergeWorker.UnshelveConflictState.MergeUnsuccessful)
                {
                    int num = (int)this.PendResolvedUnshelveConflict(conflict, MergeWorker.UnshelveConflictResolution.Automerge);
                    ListViewItem listViewItem = this.m_conflictsReverseLookup[conflict];
                    if (MergeWorker.UnshelveConflictState.MergeUnsuccessful == conflict.UnshelveConflictState)
                    {
                        listViewItem.SubItems[2].Text = "MergeFailed";
                    }
                    else
                    {
                        --this.m_unresolvedConflicts;
                        switch (conflict.UnshelveConflictState)
                        {
                            case MergeWorker.UnshelveConflictState.TakeShelved:
                                listViewItem.SubItems[2].Text = "ResolvedShelvedChange";
                                continue;
                            case MergeWorker.UnshelveConflictState.MergeSuccessful:
                                listViewItem.SubItems[2].Text = "ChangesMerged";
                                continue;
                            default:
                                listViewItem.SubItems[2].Text = "Conflict resolved";
                                continue;
                        }
                    }
                }
            }
            this.UpdateButtonStates();
        }

        private void DialogUnshelveResolve_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.m_unresolvedConflicts <= 0)
                return;
            switch (MessageBox.Show("You have not yet finished resolving all conflicts. Are you sure you want to exit?", "Unshelve", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    this.DialogResult = DialogResult.Abort;
                    break;
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        private void CreateColumns()
        {
            this.m_columnHeaders = new ColumnHeader[3];
            this.m_columnHeaders[0] = new ColumnHeader()
            {
                Text = "Name",
                Width = 130
            };
            this.m_columnHeaders[1] = new ColumnHeader()
            {
                Text = "Folder",
                Width = 250
            };
            this.m_columnHeaders[2] = new ColumnHeader()
            {
                Text = "Status",
                Width = 150
            };
            this.listViewFiles.Columns.AddRange(this.m_columnHeaders);
            this.listViewFiles.View = View.Details;
            //GuiUtil.ListViewSetSortDirectionTriangle(this.listViewFiles, this.m_columnHeaders[this.m_columnSorter.CurrentColumn], this.m_columnHeaders[this.m_columnSorter.CurrentColumn], SortOrder.Descending);
        }

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.m_columnSorter.CurrentColumn != e.Column)
            {
                //GuiUtil.ListViewSetSortDirectionTriangle(this.listViewFiles, this.m_columnHeaders[e.Column], this.m_columnHeaders[this.m_columnSorter.CurrentColumn], SortOrder.Descending);
                this.m_columnSorter.CurrentColumn = e.Column;
                this.m_columnSorter.Reverse = false;
            }
            else if (this.m_columnSorter.Reverse)
            {
                //GuiUtil.ListViewSetSortDirectionTriangle(this.listViewFiles, this.m_columnHeaders[this.m_columnSorter.CurrentColumn], this.m_columnHeaders[this.m_columnSorter.CurrentColumn], SortOrder.Descending);
                this.m_columnSorter.Reverse = false;
            }
            else
            {
                //GuiUtil.ListViewSetSortDirectionTriangle(this.listViewFiles, this.m_columnHeaders[this.m_columnSorter.CurrentColumn], this.m_columnHeaders[this.m_columnSorter.CurrentColumn], SortOrder.Ascending);
                this.m_columnSorter.Reverse = true;
            }
            this.listViewFiles.Sort();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.buttonResolve = new Button();
            this.listViewFiles = new ListView();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.buttonClose = new Button();
            this.buttonAll = new Button();
            this.labelFiles = new Label();
            this.labelSummary = new Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            this.buttonResolve.Anchor = AnchorStyles.Right;
            this.buttonResolve.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.buttonResolve.Location = new Point(115, 0);
            this.buttonResolve.Margin = new Padding(3, 0, 3, 0);
            this.buttonResolve.Name = "buttonResolve";
            this.buttonResolve.Padding = new Padding(10, 0, 10, 0);
            this.buttonResolve.Size = new Size(106, 23);
            this.buttonResolve.TabIndex = 3;
            this.buttonResolve.Text = "Resolve...";
            this.buttonResolve.UseVisualStyleBackColor = true;
            this.buttonResolve.Click += new EventHandler(this.buttonResolve_Click);
            this.listViewFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new Point(0, 41);
            this.listViewFiles.Margin = new Padding(0, 2, 0, 6);
            this.listViewFiles.MultiSelect = false;
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new Size(678, 193);
            this.listViewFiles.TabIndex = 4;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.ColumnClick += new ColumnClickEventHandler(this.listViewFiles_ColumnClick);
            this.listViewFiles.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.listViewFiles_ItemSelectionChanged);
            this.tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            this.tableLayoutPanel1.Controls.Add((Control)this.listViewFiles, 0, 2);
            this.tableLayoutPanel1.Controls.Add((Control)this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Controls.Add((Control)this.labelFiles, 0, 1);
            this.tableLayoutPanel1.Controls.Add((Control)this.labelSummary, 0, 0);
            this.tableLayoutPanel1.Location = new Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.Size = new Size(678, 269);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.Controls.Add((Control)this.buttonClose, 4, 0);
            this.tableLayoutPanel2.Controls.Add((Control)this.buttonAll, 0, 0);
            this.tableLayoutPanel2.Controls.Add((Control)this.buttonResolve, 1, 0);
            this.tableLayoutPanel2.Location = new Point(0, 246);
            this.tableLayoutPanel2.Margin = new Padding(0, 6, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            this.tableLayoutPanel2.Size = new Size(678, 23);
            this.tableLayoutPanel2.TabIndex = 0;
            this.buttonClose.Anchor = AnchorStyles.Right;
            this.buttonClose.DialogResult = DialogResult.Cancel;
            this.buttonClose.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.buttonClose.Location = new Point(569, 0);
            this.buttonClose.Margin = new Padding(3, 0, 3, 0);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Padding = new Padding(10, 0, 10, 0);
            this.buttonClose.Size = new Size(106, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
            this.buttonAll.Anchor = AnchorStyles.Right;
            this.buttonAll.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.buttonAll.Location = new Point(3, 0);
            this.buttonAll.Margin = new Padding(3, 0, 3, 0);
            this.buttonAll.Name = "buttonAll";
            this.buttonAll.Padding = new Padding(10, 0, 10, 0);
            this.buttonAll.Size = new Size(110, 23);
            this.buttonAll.TabIndex = 4;
            this.buttonAll.Text = "Auto-merge All";
            this.buttonAll.UseVisualStyleBackColor = true;
            this.buttonAll.Click += new EventHandler(this.buttonAll_Click);
            this.labelFiles.AutoSize = true;
            this.labelFiles.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.labelFiles.Location = new Point(0, 26);
            this.labelFiles.Margin = new Padding(0, 3, 0, 0);
            this.labelFiles.Name = "labelFiles";
            this.labelFiles.Size = new Size(32, 13);
            this.labelFiles.TabIndex = 6;
            this.labelFiles.Text = "Files:";
            this.labelSummary.AutoSize = true;
            this.labelSummary.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.labelSummary.Location = new Point(0, 0);
            this.labelSummary.Margin = new Padding(0, 0, 0, 10);
            this.labelSummary.Name = "labelSummary";
            this.labelSummary.Size = new Size(329, 13);
            this.labelSummary.TabIndex = 7;
            this.labelSummary.Text = "The following conflicts must be resolved to unshelve this shelveset.";
            this.AcceptButton = (IButtonControl)this.buttonClose;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = (IButtonControl)this.buttonClose;
            this.ClientSize = new Size(702, 293);
            this.Controls.Add((Control)this.tableLayoutPanel1);
            this.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.MinimumSize = new Size(600, 300);
            this.Name = nameof(DialogUnshelveResolve);
            this.Text = "Unshelve/Merge Shelveset";
            this.FormClosing += new FormClosingEventHandler(this.DialogUnshelveResolve_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private enum ListViewColumns
        {
            Name,
            Folder,
            Status,
            Last,
        }

        public delegate MergeWorker.UnshelveConflictState PendResolvedUnshelveConflictDelegate(
          MergeWorker.ShelvedItem item,
          MergeWorker.UnshelveConflictResolution newState);
    }
}
