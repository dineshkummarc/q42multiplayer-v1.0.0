using System;
using System.IO;
using Q42.Wheels.Multiplayer;

public partial class ping : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    // Disable cache
    Response.AddHeader("Pragma", "no-cache");
    Response.AddHeader("Cache-Control", "no-store");
    Response.AddHeader("Expires", "0");
    
    // ping
    ChatServer chatbox = Application["Chatbox"] as ChatServer;
    Ping ping = chatbox.Ping(Context);

    StringWriter writer = new StringWriter();
    writer.WriteLine("id:{0},", ping.MyUser.Id);
    writer.WriteLine("events:[");
    for (int i=0; i<ping.Events.Count; i++)
    {
      Event evt = ping.Events[i];
      writer.Write("[{0},\"{1}\",\"{2}\"]{3}", evt.User.Id, evt.Property.Name, evt.Property.Value, (i < ping.Events.Count - 1)? "," : "");
    }
    writer.WriteLine("]");
    Response.Write(writer.ToString());
  }
}
