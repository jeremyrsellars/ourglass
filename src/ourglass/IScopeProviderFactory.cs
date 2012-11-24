using System.Diagnostics;

namespace Ourglass
{
   public interface IScopeProviderFactory
   {
      bool SupportsProcess(Process process);
      IScopeProvider CreateScopeProvider(Process process);
   }
}
