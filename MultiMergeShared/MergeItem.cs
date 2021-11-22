using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace MultiMerge
{
    
    internal class MergeItem
    {
        public string SourcePath;
        public string DestinationPath;
        public bool IsFolder;
        public List<Changeset> Changesets = new List<Changeset>();
    }
    
}