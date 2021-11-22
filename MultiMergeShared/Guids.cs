// Guids.cs
// MUST match guids.h

using System;

namespace MultiMerge
{
    static class GuidList
    {
        public const string guidFind_Changeset_By_CommentPkgString = "25ac1559-f44d-4bce-96f5-c72e64641553";
        public const string guidFind_Changeset_By_CommentCmdSetString = "534a73b3-154d-449a-8737-54ddb29e1ff1";
        public const string guid_Merge_ChangeSet_CmdSetString = "{A8B5EBD2-7122-424E-82A3-F2357E4320B8}";
        public const string UnshelveToBranche = "{F5195F42-7105-4ED3-83C5-CBDF6CAB11CA}";
        
        public static readonly Guid guidMerge_Changeset_By_CommentCmdSet = new Guid(guidFind_Changeset_By_CommentCmdSetString);

        public static readonly Guid guid_Merge_ChangeSet_CmdSet = new Guid(guid_Merge_ChangeSet_CmdSetString);

        public static readonly Guid guid_ExcludeFromTFS = new Guid("{A89A3081-5167-4792-BAC0-130AA976F465}");
        public static readonly Guid guid_UnshelveToBranche = new Guid(UnshelveToBranche);
    };
}