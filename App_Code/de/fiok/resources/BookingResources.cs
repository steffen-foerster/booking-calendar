using System;
using System.Resources;
using System.Reflection;
namespace de.fiok.resources
{
  public class BookingResources
  {
    public BookingResources ()
    {
    }

    public static ResourceManager GetResourceManager ()
    {
      return new ResourceManager ("de.fiok.resources.BookingResources", Assembly.GetAssembly (new BookingResources ().GetType ()));
    }
  }
}
