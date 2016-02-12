// This file is automatically edited by the Rakefile, see the "write_assembly_info_product" task for details.
using System.Reflection;
using System.Resources;

[assembly: AssemblyVersion("2.5.0")] // Line written by Rakefile
[assembly: AssemblyFileVersion("2.5.0.0")] // Line written by Rakefile
[assembly: AssemblyInformationalVersion("develop")] // Line written by Rakefile

// This prevents AppDomain.AssemblyResolve events from firing for <Assembly>.resources. This is a problem
// for the nFlow assemblies which annoyingly implement their own assembly loading.
// For more information, see http://stackoverflow.com/questions/4368201/appdomain-currentdomain-assemblyresolve-asking-for-a-appname-resources-assembly
[assembly: NeutralResourcesLanguageAttribute("en-US", UltimateResourceFallbackLocation.MainAssembly)]
