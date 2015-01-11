using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace de.fiok.core
{
  public class ThreadLocal : Dictionary<Thread, object>
  {
    public void Add (object value)
    {
      if (this.ContainsKey (Thread.CurrentThread)) {
        this.Remove (Thread.CurrentThread);
      }
      this.Add (Thread.CurrentThread, value);
    }

    public void Remove ()
    {
      this.Remove (Thread.CurrentThread);
    }

    public object Get ()
    {
      return this[Thread.CurrentThread];
    }
  }
}
