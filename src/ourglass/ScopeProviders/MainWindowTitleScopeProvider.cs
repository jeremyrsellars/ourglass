using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Ourglass.ScopeProviders
{
   public class MainWindowTitleScopeProvider : IScopeProvider
   {
      readonly Process process;
      readonly Func<string, IScope> scopeInterpretter;

      public MainWindowTitleScopeProvider(Process process, Func<string, IScope> scopeInterpretter = null)
      {
         if(process == null)
            throw new ArgumentNullException("process");
         this.process = process;
         this.scopeInterpretter = scopeInterpretter ?? GetScopeFromWindowTitle;
      }

      public IScope GetCurrentScope()
      {
         process.Refresh();
         return scopeInterpretter(process.MainWindowTitle);
      }

      static readonly Regex DocumentAndAppRegex = new Regex("^(<context>.+?)\u0020*-\u0020*(?<app>.+)$", RegexOptions.RightToLeft | RegexOptions.Compiled);
      public static IScope GetScopeFromWindowTitle(string title)
      {
         return ScopeFromTitleOrNull(title) ?? NullScope.Instance;
      }

      static IScope ScopeFromTitleOrNull(string title)
      {
         var match = DocumentAndAppRegex.Match(title);
         if (match.Success)
            return new Scope { Application = match.Groups["app"].Value, Context = match.Groups["context"].Value };
         return null;
      }
   }
}
