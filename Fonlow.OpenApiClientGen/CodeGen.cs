﻿using Fonlow.OpenApiClientGen.ClientTypes;
using Microsoft.OpenApi.Models;
using System;

namespace Fonlow.CodeDom.Web
{
	public static class CodeGen
	{
		public static void GenerateClientAPIs(Settings settings, OpenApiPaths paths, OpenApiComponents components, string outputBasePath)
		{
			var currentDir = System.IO.Directory.GetCurrentDirectory();
			if (settings.ClientLibraryProjectFolderName != null)
			{
				string csharpClientProjectDir = System.IO.Path.IsPathRooted(settings.ClientLibraryProjectFolderName) ?
					settings.ClientLibraryProjectFolderName : System.IO.Path.Combine(outputBasePath, settings.ClientLibraryProjectFolderName);

				if (!System.IO.Directory.Exists(csharpClientProjectDir))
				{
					var msg = $"{csharpClientProjectDir} not exist while current directory is {currentDir}";
					throw new CodeGenException(msg);
				}
				var path = System.IO.Path.Combine(csharpClientProjectDir, settings.ClientLibraryFileName);
				var gen = new Fonlow.OpenApiClientGen.Cs.ControllersClientApiGen(settings);
				gen.CreateCodeDom(paths, components);
				gen.Save(path);
			}


			Func<string, string, string> CreateTsPath = (folder, fileName) =>
			{
				if (folder != null)
				{
					string theFolder;
					try
					{
						theFolder = System.IO.Path.IsPathRooted(folder) ?
							folder : System.IO.Path.Combine(outputBasePath, folder);

					}
					catch (ArgumentException e)
					{
						System.Diagnostics.Trace.TraceWarning(e.Message);
						var msg = $"Invalid TypeScriptFolder {folder} while current directory is {currentDir}";
						throw new CodeGenException(msg);
					}

					if (!System.IO.Directory.Exists(theFolder))
					{
						var msg = $"TypeScriptFolder {theFolder} not exist while current directory is {currentDir}";
						throw new CodeGenException(msg);
					}
					return System.IO.Path.Combine(theFolder, fileName);
				};

				return null;
			};

			if (settings.Plugins != null)
			{
				var exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				foreach (var plugin in settings.Plugins)
				{
					var jsOutput = new JSOutput
					{
						JSPath = CreateTsPath(plugin.TargetDir, plugin.TSFile),
						AsModule = plugin.AsModule,
						ContentType = plugin.ContentType,
					};

					var assemblyFilePath = System.IO.Path.Combine(exeDir, plugin.AssemblyName + ".dll");
					var tsGen = PluginFactory.CreateImplementationsFromAssembly(assemblyFilePath, settings, jsOutput);
					if (tsGen != null)
					{
						tsGen.CreateCodeDom(paths, components);
						tsGen.Save();
					}
					else
					{
						System.Diagnostics.Trace.TraceWarning($"Not done with plugin {plugin.AssemblyName}");
					}
				}
			}
		}
	}
}
