//
// ProjectExtensions.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.IO;

using MonoDevelop.IPhone;
using MonoDevelop.MonoDroid;
using MonoDevelop.Projects;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using Xamarin.AndroidTools;


namespace Apperian.Ease.Publisher
{
	public static class ProjectExtensions
	{
		public static string GetApplicationName (this DotNetProject project)
		{
			var configuration = project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);

			var iphoneconfig = configuration as IPhoneProjectConfiguration;
			if (iphoneconfig != null && !string.IsNullOrEmpty(iphoneconfig.IpaPackageName))
				return iphoneconfig.IpaPackageName;

			var androidproject = project as MonoDroidProject;
			if (androidproject != null && androidproject.AndroidManifest != FilePath.Empty && androidproject.AndroidManifest != FilePath.Null) {
				var manifest = AndroidAppManifest.Load (androidproject.AndroidManifest.FullPath);
				if (manifest != null && !string.IsNullOrEmpty (manifest.ApplicationLabel))
					return manifest.ApplicationLabel;
			}
			return project.Name;
		}

		public static string GetBuildArtifact (this DotNetProject project, DotNetProjectConfiguration configuration)
		{
			if (project is IPhoneProject && configuration is IPhoneProjectConfiguration)
				return GetIpaFilename (project as IPhoneProject, configuration as IPhoneProjectConfiguration);

			if (project is MonoDroidProject && configuration is MonoDroidProjectConfiguration)
				return (configuration as MonoDroidProjectConfiguration).ApkPath;
			return null;
		}
		
		static string GetIpaFilename (IPhoneProject proj, IPhoneProjectConfiguration conf)
		{

			var plist = Xamarin.MacDev.PDictionary.FromFile (proj.GetInfoPlistFile ().FilePath);
			var name = conf.IpaPackageName;
			if (string.IsNullOrEmpty (name)) {
				name = conf.CompiledOutputName.FileNameWithoutExtension;
				var version = plist != null ? (plist["CFBundleVersion"] as Xamarin.MacDev.PString).Value : null;
				if (!string.IsNullOrEmpty (version))
					name += "-" + version;
			}
			if (!name.EndsWith (".ipa", StringComparison.OrdinalIgnoreCase))
				name += ".ipa";
			return Path.Combine (conf.OutputDirectory, name);
		}
	}
}