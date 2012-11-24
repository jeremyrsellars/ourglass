using System;
using System.Diagnostics;

namespace Ourglass.VisualStudio
{
   public class VisualStudioScopeProviderFactory : IScopeProviderFactory
   {
      public bool SupportsProcess(Process process)
      {
         return string.Equals("devenv.exe", process.GetImage(), StringComparison.OrdinalIgnoreCase);
      }

      public IScopeProvider CreateScopeProvider(Process process)
      {
         return new VisualStudioScopeProvider(process);
      }
   }
}
