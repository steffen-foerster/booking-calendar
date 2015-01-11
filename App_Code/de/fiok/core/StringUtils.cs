using System;
using System.Collections.Generic;
using System.Text;

namespace de.fiok.core
{
  public class StringUtils
  {
    public static bool IsBlank (String value)
    {
      if (String.IsNullOrEmpty (value)) {
        return true;
      }
      if ((value.Trim ()).Length == 0) {
        return true;
      }
      return false;
    }

    public static bool IsNotBlank (String value)
    {
      return !IsBlank (value);
    }
  }
}
