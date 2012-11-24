using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinUIScraper.Declarative;
using WinUIScraper.Providers;
using WinUIScraper.Providers.UIAutomation;
using System.Windows.Automation;

namespace Ourglass.VisualStudio
{
   public class VisualStudioScopeProvider : IScopeProvider
   {
      readonly Process process;
      const string FileWithKeyboardFocus = "fileWithKeyboardFocus";
      const string FileFromFirstDocumentGroup = "fileFromFirstDocumentGroup";

      public VisualStudioScopeProvider(Process process)
      {
         this.process = process;
      }

      public IScope GetCurrentScope()
      {
         var values = CreateHierarchicalValueProvider(GetMainWindowElement()).GetValues(BuildVisualStudio2010Tree());
         return CreateScopeFromValues(values);
      }

      static IScope CreateScopeFromValues(Dictionary<string, List<string>> values)
      {
         List<string> scopes;
         string scope;
         if (values.TryGetValue(FileWithKeyboardFocus, out scopes) ||
             values.TryGetValue(FileFromFirstDocumentGroup, out scopes))
         {
            scope = scopes.First();
         }
         else
            scope = values.Values.SelectMany(kvp => kvp).FirstOrDefault(s => !string.IsNullOrEmpty(s));

         return CreateScope(scope);
      }

      AutomationElement GetMainWindowElement()
      {
         return AutomationElement.FromHandle(process.MainWindowHandle);
      }

      static HierarchicalValueProvider<AutomationElement, string, string> CreateHierarchicalValueProvider(AutomationElement element)
      {
         return new HierarchicalValueProvider<AutomationElement, string, string>(element);
      }

      static Node BuildVisualStudio2010Tree()
      {
         return
            new AnonymousNode(DescendentDocumentGroups())
               {
                  new AnonymousNode(GetKeyboardFocusElement())
                  {
                     new ValueNode(FileWithKeyboardFocus, GetParentTabItem(), GetName),
                  },
                  new AnonymousNode(GetSelectedChildren())
                  {
                     new ValueNode(FileFromFirstDocumentGroup, GetSameElement, GetName),
                  }
               };
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> DescendentDocumentGroups()
      {
         return element => element.FindDescendantsByControlType(ControlType.Tab).Where(e => "DocumentGroup" == e.GetClassName());
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> GetParentTabItem()
      {
         return element => new [] {element.FindFirstAncestorByControlType(ControlType.TabItem)};
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> GetSelectedChildren()
      {
         return element => element.FindChildrenBy(SelectionItemPattern.IsSelectedProperty, true);
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> GetKeyboardFocusElement()
      {
         return element => element.FindDescendantsBy(AutomationElement.HasKeyboardFocusProperty, true);
      }

      static IEnumerable<AutomationElement> GetSameElement(AutomationElement element)
      {
         return new[] { element };
      }

      static string GetName(AutomationElement element)
      {
         var s = element.GetName();
         return s ?? "Didn't happen this time";
      }


      static IScope CreateScope(string context)
      {
         return new Scope {Application = "Microsoft Visual Studio", Context = context};
      }

      class Node : KeyedTreeNode<string, IElementsProvider<AutomationElement, string>>
      {
         protected Node(string key, Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement, string> dataSelector)
            : base(key, new AutomationElementsProvider(sourcesSelector, dataSelector))
         {
         }
      }
      class ValueNode : Node
      {
         public ValueNode(string key, Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement, string> dataSelector)
            : base(key, sourcesSelector, dataSelector)
         {
         }
      }
      class AnonymousNode : Node
      {
         public AnonymousNode(Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector)
            : base(null, sourcesSelector, null)
         {
         }
      }
      class AutomationElementsProvider : DelegatedElementsProvider<AutomationElement, string>
      {
         public AutomationElementsProvider(Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement, string> dataSelector = null)
            : base(sourcesSelector, dataSelector)
         {
         }
      }
   }

   static class DeclarativeExtensions
   {
      public static Func<AutomationElement, IEnumerable<AutomationElement>> Where(this Func<AutomationElement, IEnumerable<AutomationElement>> func, Func<AutomationElement, bool> predicate)
      {
         return elem => func(elem).Where(predicate);
      }

      public static string GetClassName(this AutomationElement ae)
      {
         return (string)ae.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
      }
   }
}
