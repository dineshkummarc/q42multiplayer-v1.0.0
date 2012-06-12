using System;
using Q42.Wheels.Multiplayer;

public partial class status : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    // Disable cache
    Response.AddHeader("Pragma", "no-cache");
    Response.AddHeader("Cache-Control", "no-store");
    Response.AddHeader("Expires", "0");

    ChatServer chatbox = Application["Chatbox"] as ChatServer;
    Response.Write(String.Format("<p>Rooms: {0}</p>", chatbox.Rooms.Count));
    foreach (Room room in chatbox.Rooms.Values)
    {
      Response.Write(String.Format("<b>Room: {0}, Users: {1}</b><br/>", room.Name, room.Users.Count));
      foreach (User user in room.Users)
      {
        Response.Write(String.Format("User: {0}, events in queue: {1}<br/>", user.Id, user.Events.Count));
        foreach (Property prop in user.Properties)
        {
          Response.Write(String.Format("- {0} = {1}<br/>", prop.Name, prop.Value));
        }
      }
      Response.Write("<hr/>");
    }
  }
}
