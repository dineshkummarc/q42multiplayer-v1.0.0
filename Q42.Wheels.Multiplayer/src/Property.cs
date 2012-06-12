/*
$Id$
$Revision$
$Author$
$Date$
*/
using System;

namespace Q42.Wheels.Multiplayer
{
  /// <summary>
  /// A name/value property with timestamp. When made persistent, it can be considered a
  /// user property. When made non-persistent, it can be considered as an Event.
  /// </summary>
  public class Property
  {
    private bool persistent = false;
    private string name;
    private string value;
    private DateTime lastModified;

    /// <summary>
    /// Creates a new property and marks it as modified.
    /// </summary>
    public Property()
    {
      lastModified = DateTime.Now;
    }

    /// <summary>
    /// Creates a property by name and value, and marks it as modified.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="value">Value of the property.</param>
    public Property(string name, string value)
      : this()
    {
      this.name = name;
      this.value = value;
    }

    /// <summary>
    /// Creates a property by name, value and persistency, and marks it as modified.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="value">Value of the property.</param>
    /// <param name="persistent">Makes the property persistent or not.</param>
    public Property(string name, string value, bool persistent)
      : this(name, value)
    {
      this.persistent = persistent;
    }

    /// <summary>
    /// Sets or gets the persistency value. If the persistent is set to true, 
    /// new users within a room should always be notified of its current value.
    /// </summary>
    public bool Persistent
    {
      get { return persistent; }
      set { persistent = value; }
    }

    /// <summary>
    /// Name of the property.
    /// </summary>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// Value of the property.
    /// </summary>
    public string Value
    {
      get { return value; }
      set
      {
        this.value = value;
        lastModified = DateTime.Now;
      }
    }

    /// <summary>
    /// Timestamp of the last modification to this property's value.
    /// </summary>
    public DateTime LastModified
    {
      get { return lastModified; }
      set { lastModified = value; }
    }
  }
}