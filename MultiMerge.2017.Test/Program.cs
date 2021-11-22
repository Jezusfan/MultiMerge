using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using MultiMerge;

namespace FindChangeByComment.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                var teamProjectCollection = new TfsTeamProjectCollection(new Uri("http://tfs:8080/tfs"));
                teamProjectCollection.Authenticate();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                string path = null, changeset = null;
                if (Directory.Exists(@"C:\TFS"))
                    path = @"C:\TFS";
                else if (Directory.Exists(@"D:\TFS"))
                    path = @"D:\TFS";

                var logger = new TestLogger();
                if (InputDialog.Show("Input workspace path", "Test Multi Merge", ref path) ==
                    DialogResult.OK)
                {
                    
                    var versionControlServer = teamProjectCollection.GetService<VersionControlServer>();
                    var workspace = versionControlServer.GetWorkspace(path);
    #if DEBUG

                    if (InputDialog.Show("Optional: input changeSet Id", "Test Multi Merge", ref changeset) == DialogResult.OK && !string.IsNullOrEmpty(changeset))
                        Application.Run(new MergeUI(versionControlServer, workspace, logger, path, int.Parse(changeset)));
                    else
                        Application.Run(new MergeUI(versionControlServer, workspace, logger, path));
    #endif
                }
                
            }
            catch (Exception ex)
            {
                MultiMerge.MessageBox.Show(ex.ToString(), "Error");
            }
            
        }
    }
}
