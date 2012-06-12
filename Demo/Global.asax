<%@ Application Language="C#" %>

<script RunAt="server">

  void Application_Start(object sender, EventArgs e)
  {
    Application["Chatbox"] = new ChatServer();
  }

  void Session_End(object sender, EventArgs e)
  {
    ((ChatServer)Application["Chatbox"]).OnSessionEnd(Context);
  }
       
</script>

