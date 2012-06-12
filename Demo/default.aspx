<html>
  <head>
    <script src="js/Multiplayer.js"></script>
    <script src="js/ChatClient.js"></script>
    <style>
    #log {
      width: 400px; height: 200px;
    }
    #msg {
      width: 400px;
    }
    #chat
    {
      display: none;
    }
    </style>
  </head>
  <body>
    <form id="joinForm" onsubmit="ChatClient.join(); return false;" >
      Name: <input id="name" type="text" />
      <input type="submit" value="join">
    </form>
    <div id="chat">
      <textarea id="log"></textarea>
      <form onsubmit="ChatClient.sendMessage(); return false;" >
        <input id="msg" type="text" />
        <input type="submit" value="send">
      </form>
    </div>
  </body>
</html>