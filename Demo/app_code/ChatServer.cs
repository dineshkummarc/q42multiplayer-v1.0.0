using System.Collections.Specialized;
using System.Web;
using Q42.Wheels.Multiplayer;

public class ChatServer : Server
{
  public override bool MultipleUsersPerSession
  {
    get { return true; }
  }
  
  public override int MaxUsersPerRoom
  {
    get { return 5; }
  }

  public override int MaxPingInterval
  {
    get { return 10; }
  }

  public override NameValueCollection GetInput(HttpContext context)
  {
    return context.Request.QueryString;
  }

  public override Property ValidateProperty(string name, string value)
  {
    Property prop = null;
    switch (name)
    {
      case "name":
        prop = new Property(name, value, true);
        break;
      case "say":
        if (value != "poep")
          prop = new Property(name, value, false);
        break;
    }
    return prop;    
  }
}