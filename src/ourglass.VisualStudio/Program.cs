using System;
using System.Diagnostics;
using System.Linq;

namespace Ourglass.VisualStudio
{
   static class Program
   {
      static void Main()
      {
         var process = Process.GetProcessesByName("devenv").First();
         var factory = new VisualStudioScopeProviderFactory();
         while (factory.SupportsProcess(process))
         {
            DumpScope(factory.CreateScopeProvider(process).GetCurrentScope());
            System.Threading.Thread.Sleep(1000);
         }
      }

      static void DumpScope(IScope scope)
      {
         if (scope == null)
            Console.WriteLine("Null Scope" + Environment.NewLine);
         else
            Console.WriteLine(scope.Application + Environment.NewLine + scope.Context + Environment.NewLine);
      }
   }
}
