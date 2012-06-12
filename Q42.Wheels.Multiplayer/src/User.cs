/*
$Id$
$Revision$
$Author$
$Date$
*/
using System;
using System.Collections.Generic;

namespace Q42.Wheels.Multiplayer
{
  /// <summary>
  /// A User is a single client connection within a room. A user can store any number
  /// of name/value properties, and collects its own list of Events that needs to be
  /// sent down to the client at every Ping.
  /// </summary>
  public class User
  {
    /// <summary>
    /// Static counter to set new id's that auto increment.
    /// </summary>
    private static int idCounter = 0;

    private readonly int id;
    
    // used internally
    private readonly Dictionary<string, Property> propertyDict;
    
    // used external
    private readonly List<Property> propertyList;
    private List<Event> eventList;
    private DateTime lastModified;
    private DateTime lastPing;
    private Room room;
    
    /// <summary>
    /// Creates a new User and generates its id.
    /// </summary>
    public User()
    {
      // generate the id
      id = idCounter++;
      
      // create the property collections
      eventList = new List<Event>();
      propertyDict = new Dictionary<string, Property>();
      propertyList = new List<Property>();

      // mark the user as modified, because it is new
      lastModified = DateTime.Now;
      
      // set this user's last ping to now
      lastPing = DateTime.Now;
      
    }
    
    /// <summary>
    /// Id of the User.
    /// </summary>
    public int Id
    {
      get { return id; }
    }

    /// <summary>
    /// Sets a name/value property and creates it if it doesn't exist.
    /// Also sets this User's LastModified date.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="value">Value of the property.</param>
    /// <returns>The property object that was set.</returns>
    public Property SetProperty(string name, string value)
    {
      Property prop = null;
      if (propertyDict.ContainsKey(name))
      {
        prop = propertyDict[name];
        prop.Value = value;
        lastModified = DateTime.Now;
      }
      else
      {
        prop = new Property(name, value);
        // add it to our dictionary by its name, so we can quickly check for it
        propertyDict.Add(name, prop);
        // add it to the list for public use
        propertyList.Add(prop);
        lastModified = DateTime.Now;
      }
      return prop;
    }

    /// <summary>
    /// Timestamp of the last modification to any property of this User.
    /// </summary>
    public DateTime LastModified
    {
      get { return lastModified; }
      set { lastModified = value; }
    }

    /// <summary>
    /// Timestamp of the previous ping that this user has performed.
    /// </summary>
    public DateTime LastPing
    {
      get { return lastPing; }
      set { lastPing = value; }
    }

    /// <summary>
    /// List of all properties belonging to this user.
    /// </summary>
    public List<Property> Properties
    {
      get { return propertyList; }
    }

    /// <summary>
    /// List of all events ready for shipping at the next ping.
    /// </summary>
    public List<Event> Events
    {
      get { return eventList; }
    }
    
    /// <summary>
    /// The Room this User is in.
    /// </summary>
    public Room Room
    {
      get { return room; }
      set { room = value; }
    }
  }
}