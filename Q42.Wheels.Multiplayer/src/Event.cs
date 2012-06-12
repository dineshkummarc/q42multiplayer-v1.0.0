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
  /// The Event class represents a User's property change. It holds 
  /// a reference to the specific User, and a Property of what 
  /// has been changed. Events are collected per User, and when that
  /// User pings, its Event list is cleared.
  /// </summary>
  public class Event
  {
    private readonly User user;
    private readonly Property property;
    
    /// <summary>
    /// Creates a new property change Event instance for the give User
    /// and the given Property.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="property"></param>
    public Event(User user, Property property)
    {
      this.user = user;
      this.property = property;
    }

    /// <summary>
    /// The User of which the Property was changed.
    /// </summary>
    public User User
    {
      get { return user; }
    }

    /// <summary>
    /// The Property that was changed.
    /// </summary>
    public Property Property
    {
      get { return property; }
    }
  }
}