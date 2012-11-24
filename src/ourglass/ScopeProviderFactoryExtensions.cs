using System.Diagnostics;
using System.IO;

namespace Ourglass
{
   public static class ScopeProviderFactoryExtensions
   {
      public static string GetImage(this Process process)
      {
         return Path.GetFileName(process.MainModule.FileName);
      }
   }
}