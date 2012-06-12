/*
$Id$
$Revision$
$Author$
$Date$
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace Q42.Wheels.Multiplayer
{
  /// <summary>
  /// A Ping instance is created every time the browser polls the server.
  /// Ping takes care of the property modifications for the current User, and
  /// holds list of Events to notify this user of other User's changes. 
  /// </summary>
  public class Ping
  {
    private readonly HttpContext context;
    private readonly Server server;
    private readonly List<Event> events;
    private bool isNewUser = false;

    /// <summary>
    /// Gets my User instance.
    /// </summary>
    private User myUser
    {
      get
      {
        return server.GetMyUser(context);
      }
    }

    /// <summary>
    /// Gets the Room instance that myUser is in.
    /// </summary>
    private Room myRoom
    {
      get
      {
        return server.GetMyRoom(context);
      }
    }

    /// <summary>
    /// Handles all incoming modifications to myUser.
    /// </summary>
    private void parseInput()
    {
      // get all names in the current request
      foreach (string name in server.GetInput(context))
      {
        // if myUser is disconnecting, let the others know
        if (name == "disconnect")
        {
          // the Remove method takes care of all DisconnectEvent stuff
          myRoom.RemoveUser(myUser);
          break;
        }

        // otherwise, get the value and parse the property
        string[] values = server.GetInput(context).GetValues(name);
        foreach (string value in values)
        {
          Property prop = server.ValidateProperty(name, value);
          
          // if the property is valid
          if (prop != null)
          {
            // set persistent properties
            if (prop.Persistent)
              myUser.SetProperty(prop.Name, prop.Value);
            // and dispatch non-persistent properties as events
            else
              myRoom.DispatchEvent(new Event(myUser, prop));  
          }
        }
      }
    }

    /// <summary>
    /// Build up the list of property change events of other Users in myRoom.
    /// </summary>
    private void getEvents()
    {
      // get all events delivered to my user, and clear its list
      events.AddRange(myUser.Events);
      myUser.Events.Clear();
      
      // get changed user properties and make events of them
      foreach (User user in myRoom.Users)
      {
        // ignore myself
        if (user == myUser)
          continue;

        // if myUser is new to myRoom, get notified of everything, 
        // otherwise only handle properties changed since my last ping
        if (isNewUser || user.LastModified > myUser.LastPing)
          foreach (Property prop in user.Properties)
            if (isNewUser  || prop.LastModified > myUser.LastPing)
              events.Add(new Event(user, prop));
      }
    }

    
    /// <summary>
    /// Make default constructor unavailable.
    /// </summary>
    protected Ping()
    {
    }
    
    /// <summary>
    /// Creates a new Ping instance for the given HttpContext.
    /// Internal constructor. Use MuliUser.Application.Ping to invoke.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    /// <param name="server">An Multiplayer server instance.</param>
    internal Ping(HttpContext context, Server server)
    {
      // set app, context and create event lists
      this.server = server;
      this.context = context;
      events = new List<Event>();
      
      // if myUser has switched rooms
      if (myUser.Room != null && myUser.Room != myRoom)
      {
        // notify my old roommates of my disconnection
        myUser.Room.RemoveUser(myUser);
        if (myUser.Room.Users.Count == 0)
          server.Rooms.Remove(myUser.Room.Name);
        else
        {
          // but don't forget to send myself the disconnection event of all my previous roommates!
          foreach (User user in myUser.Room.Users)
            events.Add(new Event(user, new Property("disconnect", "true", false)));
        }
          
      }
      // if myUser is new in the room, mark it as new
      if (!myRoom.ContainsUser(myUser))
      {
        myRoom.AddUser(myUser);
        isNewUser = true;
        
        // and mark me and my properties as changed
        myUser.LastModified = DateTime.Now;
        foreach (Property prop in myUser.Properties)
          prop.LastModified = DateTime.Now;
      }
      
      // parse myUser's modifications that were passed with the request
      parseInput();
      
      // now get all other Users' modifications
      getEvents();
      
      // set lastPing
      myUser.LastPing = DateTime.Now;
      
      // remove my room it is empty
      if (myRoom.Users.Count == 0)
        server.Rooms.Remove(myRoom.Name);
    }

    /// <summary>
    /// A list of property change Events that the current User
    /// needs to be aware of.
    /// </summary>
    public List<Event> Events
    {
      get { return events; }
    }

    /// <summary>
    /// Me
    /// </summary>
    public User MyUser
    {
      get { return myUser; }
    }

    /// <summary>
    /// The room that this User is currently in.
    /// </summary>
    public Room MyRoom
    {
      get { return myRoom; }
    }
  }
}