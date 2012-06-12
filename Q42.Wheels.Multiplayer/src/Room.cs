/*
$Id$
$Revision$
$Author$
$Date$
*/
using System.Collections.Generic;

namespace Q42.Wheels.Multiplayer
{
  /// <summary>
  /// A Room is a virtual area that holds any number of Users that are currently
  /// "present". 
  /// </summary>
  public class Room
  {
    /// <summary>
    /// Static counter to set new id's that auto increment.
    /// </summary>
    private static int idCounter = 0;

    private readonly int id;
    private readonly string name;
    private readonly List<User> users;
    
    /// <summary>
    /// Creates a new Room instance.
    /// </summary>
    public Room(string name)
    {
      // generate the id, users and disconnectEvents objects
      id = idCounter++;
      this.name = name;
      users = new List<User>();
    }

    /// <summary>
    /// Id of the room.
    /// </summary>
    public int Id
    {
      get { return id; }
    }

    /// <summary>
    /// Name of the room.
    /// </summary>
    public string Name
    {
      get { return name; }
    }

    /// <summary>
    /// List of Users currently present in this room.
    /// </summary>
    public List<User> Users
    {
      get { return users; }
    }

    /// <summary>
    /// Adds a new user to the room.
    /// </summary>
    /// <param name="user">User to add.</param>
    public void AddUser(User user)
    {
      if (!users.Contains(user))
      {
        users.Add(user);
        user.Room = this;
        // clear optional events of a previous room
        user.Events.Clear();
      }
    }

    /// <summary>
    /// Removes a user from the list and adds a DisconnectEvent to handle
    /// the notification to other users within this room.
    /// </summary>
    /// <param name="user">User to remove.</param>
    public void RemoveUser(User user)
    {
      if (user != null && users.Contains(user))
      {
        users.Remove(user);
        // create the DisconnectEvent and add it to this room's list.
        DispatchEvent(new Event(user, new Property("disconnect", "true", false)));
      }
    }
    
    /// <summary>
    /// Returns true if the requested User is currently present in this room.
    /// </summary>
    /// <param name="user">User to check.</param>
    /// <returns>True if the requested User is currently present in this room.</returns>
    public bool ContainsUser(User user)
    {
      return users.Contains(user);
    }
    
    /// <summary>
    /// Dispatches an event to all Users in this Room.
    /// </summary>
    /// <param name="evt"></param>
    public void DispatchEvent(Event evt)
    {
      foreach (User user in users)
        if (user != evt.User)
          user.Events.Add(evt);
    }
  }
}