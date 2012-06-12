/// <reference path="Multiplayer.js" />
var ChatClient =
{
  users: {},
  join: function() {    
    ChatClient.name = document.getElementById("name").value;
    document.getElementById("joinForm").style.display = "none";
    document.getElementById("chat").style.display = "block";
    Multiplayer.init("ping.aspx", 2000, ChatClient.handleEvent, { "name": true, "say": false });
    var roomName = document.location.search.replace(/.*?room=(.*)/g, '$1');
    roomName = (roomName == document.location.search) ? 0 : roomName;
    Multiplayer.connect({ "name": ChatClient.name }, roomName);
    ChatClient.writeLog("* " + ChatClient.name + " joined.");
  },
  handleEvent: function(userId, name, value) {
    if (!ChatClient.users[userId]) ChatClient.users[userId] = {};
    var user = ChatClient.users[userId];
    user[name] = value;
    if (name == "disconnect") ChatClient.writeLog("* " + user.name + " disconnected.");
    if (name == "name") ChatClient.writeLog("* " + value + " joined.");
    if (name == "say") ChatClient.writeLog((user.name ? user.name : userId) + ": " + value);
  },
  sendMessage: function() {
    var text = document.getElementById("msg").value;
    if (text.indexOf("room") == 0)
      Multiplayer.send({ "room": text.substr(5) });
    else
      Multiplayer.send({ "say": text });
    ChatClient.writeLog(ChatClient.name + ": " + text);
    document.getElementById("msg").value = "";
  },
  writeLog: function(msg) {
    document.getElementById("log").value += msg + "\n";
    document.getElementById("log").scrollTop = document.getElementById("log").scrollHeight;
  }
};