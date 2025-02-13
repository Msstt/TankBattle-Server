using System.Net.Sockets;

public class EventHandler {
  public static void OnDisconnect(ClientState client) {
    BrawlServer.SendToOther(client, "Leave", new string[] {
      client.socket.RemoteEndPoint.ToString()
    });
  }
}
