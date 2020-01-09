using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SlonResourcesDiagnostika
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		void App_Startup(object sender, StartupEventArgs e)
		{
			//This is used to load the assembly from the resources instead of having it as standalone dll
			AppDomain.CurrentDomain.AssemblyResolve += (se, args) =>
			{
				Assembly thisAssembly = Assembly.GetExecutingAssembly();

				//Get the Name of the AssemblyFile
				var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

				//Load form Embedded Resources - This Function is not called if the Assembly is in the Application Folder
				var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(name));
			    var arrResources = resources as string[] ?? resources.ToArray();
			    if (arrResources.Any())
				{
					var resourceName = arrResources.First();
					using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
					{
						if (stream == null) return null;
						var block = new byte[stream.Length];
						stream.Read(block, 0, block.Length);
						return Assembly.Load(block);
					}
				}
				return null;
			};

			new MainWindow().Show();
		}
	}
}
