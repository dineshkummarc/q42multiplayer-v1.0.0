/*
$Id$
$Revision$
$Author$
$Date$
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Timers;

namespace Q42.Wheels.Multiplayer
{
  /// <summary>
  /// Abstract class to represent a Multiplayer server. Offers a
  /// mechanisms to determine how to create what property 
  /// name/value pairs are allowed.
  /// </summary>
  public abstract class Server
  {
    private Dictionary<string, Room> rooms = new Dictionary<string,Room>();

    /// <summary>
    /// Instantiates the server instance and creates a cleanup timer
    /// </summary>
    public Server()
    {
      Timer timer = new Timer();
      timer.Interval = 1000;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(CleanupInterval);
      timer.Enabled = true;
    }
    
    /// <summary>
    /// Allow multiple users within the same session or not
    /// </summary>
    public abstract bool MultipleUsersPerSession { get; }
    
    /// <summary>
    /// Maximum number of users in a room, before scaling up to a new room
    /// </summary>
    public abstract int MaxUsersPerRoom { get; }

    /// <summary>
    /// The maximum interval in seconds between two pings of a user. If more
    /// seconds pass after its last ping, the user will be disconnected.
    /// </summary>
    public abstract int MaxPingInterval { get; }

    /// <summary>
    /// Returns the collection of name/value pairs that holds
    /// a User's modifications. Usually this is the QueryString
    /// or Forms collection.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract NameValueCollection GetInput(HttpContext context);

    /// <summary>
    /// Use the ValidateProperty handler to:
    /// - Validate if a property name is allowed
    /// - Validate if a property value is allowed, or needs to be filtered
    /// - Set persistence on the property or not
    /// </summary>
    /// <param name="name">Name of the property to set.</param>
    /// <param name="value">Value of the property to set.</param>
    /// <returns>A Property (if ok) or null (if not ok)</returns>
    public abstract Property ValidateProperty(string name, string value);

    /// <summary>
    /// Retrieves the User object that represents myself.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    /// <returns>My User instance.</returns>
    public User GetMyUser(HttpContext context)
    {
      // first check if a User is known for the current request
      User user = context.Items["Multiplayer.User"] as User;
      if (user != null)
        return user;
        
      // for multiple users per session, check user based on id
      if (MultipleUsersPerSession)
      {
        // check if an id was passed in the request
        string id = GetInput(context)["id"];        
        if (!String.IsNullOrEmpty(id))
        {
          user = context.Session["Multiplayer.User." + id] as User;
          if (user == null)
            throw new Exception("User id " + id + " was not found in your session.");
        }
        if (user == null)
        {
          user = new User();
          context.Session["Multiplayer.User." + user.Id] = user;
        }
      }     
      // if only 1 user per session is allowed 
      else
      {
        user = context.Session["Multiplayer.User"] as User;
        if (user == null)
          context.Session["Multiplayer.User"] = user = new User();
      }
      
      // store the user for this request
      context.Items["Multiplayer.User"] = user;
      return user;
    }

    /// <summary>
    /// Retrieves the Room object that my User is in.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    /// <returns>My User's Room instance.</returns>
    public Room GetMyRoom(HttpContext context)
    {
      Room room = null;
      
      // check if a room was specified, this could mean a room change
      string roomId = GetInput(context)["room"];
      if (String.IsNullOrEmpty(roomId))
      {
        // if no room was specified and my user is known to be in a room\
        room = GetMyUser(context).Room;
        if (room != null)
          return room;

        // otherwise, no room was specified so use the default name
        roomId = "default";
      }
        
      int count = 1;
      while (room == null)
      {
        // build the room with an affix, such as "bar_1" or "bar_2"
        string id = roomId + "_" + count++;
        
        // if the room exists, check if it has reached the maximum amount of users
        if (rooms.ContainsKey(id))
        {
          Room temp = rooms[id];
          if (temp.Users.Count < MaxUsersPerRoom)
            room = temp;
        }
        // otherwise create a new room by this id
        else
        {
          room = new Room(id);
          rooms.Add(id, room);
        }
      }
      
      return room;
    }
        
    /// <summary>
    /// Make sure this method is called upon the Session_End handler
    /// in Global.asax. It makes sure that when a session ends, the
    /// User for that session ends properly, and other Users get
    /// notified about it.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    public void OnSessionEnd(HttpContext context)
    {
      // if 1 user per session is allowed, remove it
      if (!MultipleUsersPerSession)
      {
        User user = GetMyUser(context);
        Room room = user.Room;
        room.RemoveUser(user);
        if (room.Users.Count == 0)
          rooms.Remove(room.Name);
      }
      // otherwise get all users and remove them
      else
      {
        foreach (object obj in context.Session)
        {
          User user = (User)obj;
          if (user != null)
          {
            Room room = user.Room;
            room.RemoveUser(user);
            if (room.Users.Count == 0)
              rooms.Remove(room.Name);
          }
        }
      }
    }
    
    /// <summary>
    /// Returns the Ping instance for the current HttpContext.
    /// Ping can process incoming User modifications and return lists
    /// of other Users' modifications and/or disconnection events.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    /// <returns>The Ping instance with references to all </returns>
    public Ping Ping(HttpContext context)
    {
      return new Ping(context, this);
    }
    
    /// <summary>
    /// Returns the list of rooms currently active
    /// </summary>
    public Dictionary<string, Room> Rooms
    {
      get { return rooms; }
    }
    
    
    /// <summary>
    /// Scheduled timer to check for users that have not pinged for more than a
    /// given amount of seconds.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CleanupInterval(object sender, ElapsedEventArgs e)
    {
      try
      {
        foreach (Room room in this.rooms.Values)
        {
          foreach (User user in room.Users)
          {
            if (user.LastPing.AddSeconds(MaxPingInterval) < DateTime.Now)
              room.RemoveUser(user);
          }
        }
      }
      catch { }
    }
  }
}