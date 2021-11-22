using System.Collections.Generic;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace MultiMerge
{
    public partial class frmMerge : MergeUI
    {
        public frmMerge(ILogger logger) : base(logger)
        {
            
        }

        public frmMerge(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger) : base(versionControl, teamExplorer, logger)
        {}

        public frmMerge(VersionControlExt versionControl, ITeamExplorer teamExplorer, ILogger logger,
            string path, List<int> changeSetids) : base(versionControl, teamExplorer, logger, path, changeSetids)
        {

        }
    }
}
