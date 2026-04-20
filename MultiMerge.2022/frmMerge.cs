using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace MultiMerge
{
    public partial class frmMerge : MergeUI
    {
        public frmMerge(ILogger logger) : base(logger)
        {
            
        }

        public frmMerge(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger, DTE2 dte = null) : base(versionControl, teamExplorer, logger, dte)
        {}

        public frmMerge(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger,
            string path, List<int> changeSetids) : base(versionControl, teamExplorer, logger, path, changeSetids)
        {

        }
    }
}
