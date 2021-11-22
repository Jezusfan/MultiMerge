# MultiMerge
Visual Studio Extension for merging individual changeset between branches

## History
I started developing this as a Visual Studio 2017 extension. I later added the option to publish a Visual Studio 2019 version of the plugin. The main bulk of development is done using VS2017. The test project to start the plugin without Visual Studio, is therefore also targeting Visual 2017-versioned dlls.

I don't have access to Team Foundation Studio at the moment, so can't refactor the test project to target Visual Studio 2019 to continue development.

I'm making the code public in the hope this will allow other developers to pick up where I left off. Making a VS2022 version should be fairly simple as the project is already setup to support multiple versions using a single solution.

## Development guide
I'm writing this from memory as it's been over a year since I last worked on this.

There is one solution, which contains all projects, targeting different versions of Visual Studio. There is a shared MultiMerge-project, two versioned projects which each referece the shared project, and a test project. 

The Merge-UI and merge-logic reside in the shared project, so any change will automatically apply to both Visual Studio plugin versions. 
>The MergeUI is a Form, inherited by a form in both 2017/2019-plugin-projects to share the actual UI code between plugins.

The hard part about this is making UI changes. In Visual Studio, shared projects do not fully support having WinForm-files, so editing Winform-UI can't be done by simply double clicking the Form. To enable the Winforms-editor, I used to copy the form-UI-files to the MultiMerge.2017-project (which also means edit the .proj-file to treat them as designer files). I then could edit the UI using thedefault WinForm editor, test it using the test project and copy the edited UI-files back to the shared project (and remove them from the VS2017-project). After that, I could simply open the solution using VS2019, compile the MultiMerge.2019-project in release-mode, which would get me the vsix-2019 installer to upload to the Visual Studio Extension Marketplace.

## Testing changes
As I mentioned, to test changes you can use the MultiMerge.2017.test-project, by setting that as the startup project, and hit 'start'. You'll need to point to a TFS-mapped folder to start with, as it will fill in the 'from-branch-folder' of the UI. It will target the  C:\TFS-folder by default, but will allow user input before it continues.

From there, getting TFS changesets works the same as from Visual Studio TFS explorer. Testing Visual Studio interaction can only be done by publishing the MultiMerge.2017/19 project and installing the produced vsix installer. 

> Note: you need to specify a newer version of the plugin, than the existing extension has, for Visual Studio to accept the manual install.

Duplicating the test.2017-project and target VS2019 libraries should be fairly easy, but since I'm not able to access TFS anymore, I leave this to other contributors.

## Versioning
Versioning the plugin has to done at two places, end keeping them in sync: 
1. the AssemblyInfo.cs file in the Properties-folder of the MultiMergeShared-folder contains the assembly-version. This needs to be updated to allow the vsix-installer to overwrite the old dll with a new version. This need to be kept in sync with nr2:
2. the source.extension.vsixmanifest, which lives in each individual MultiMerge.2017/2019-project, contains the package version. This version defines the extension-version which needs to be updated to allow Visual Studio Extension mechanism to identify the vsix installer file as an upgrade of previous package. You should change this for both 2017 and 2019, so in two places, but not sure if supporting 2017 is worth the upload.
>Developer community: feel free to add a VS2022-project file in the same way as the other projects :)

## Contributing
If you have the means to fix bugs, improve, or creating support for VS2022, I'm more then happy to help and release an update. I would need to upload a new vsix-file to the Microsoft Market Place to allow others to benefit from the change using VS extensions. Feel free to create a PR, and I'll be in touch about next steps!

Regards,

Jesusfan
