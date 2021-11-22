using Microsoft.TeamFoundation.VersionControl.Client;

namespace MultiMerge
{

    /// <summary>
    /// The search criteria for finding changesets
    /// </summary>
    class SearchCriteria
    {
        /// <summary>
        /// Gets or sets the file/folder to search
        /// </summary>
        public string SearchFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user who made the change
        /// </summary>
        public string SearchUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateVersionSpec FromDateVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateVersionSpec ToDateVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment that we're searching for
        /// </summary>
        public string SearchComment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are using regular expressions
        /// </summary>
        public bool UseRegex
        {
            get;
            set;
        }

        public bool KeepPreviousChangeSets { get; set; }
        public bool IncludeRelevantChangeSets { get; set; }
    }
}
