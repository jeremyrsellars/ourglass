namespace Ourglass
{
   public class NullScope : IScope
   {
      public static readonly NullScope Instance = new NullScope();

      NullScope()
      {
      }

      public string Application
      {
         get { return null; }
      }

      public string Context
      {
         get { return null; }
      }
   }
}