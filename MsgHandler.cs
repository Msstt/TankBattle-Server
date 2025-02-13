using System.Net.Sockets;

public class MsgHandler {
  public static void Enter(ClientState client, string[] args) {
    client.x = float.Parse(args[1]);
    client.y = float.Parse(args[2]);
    client.z = float.Parse(args[3]);
    client.v = float.Parse(args[4]);

    BrawlServer.SendToOther(client, "Enter", args);
  }

  public static void List(ClientState client, string[] args) {
    args = new string[BrawlServer.clients.Count * 6];
    int i = 0;
    foreach (ClientState otherClient in BrawlServer.clients.Values) {
      args[i] = otherClient.socket.RemoteEndPoint.ToString();
      args[i + 1] = otherClient.x.ToString();
      args[i + 2] = otherClient.y.ToString();
      args[i + 3] = otherClient.z.ToString();
      args[i + 4] = otherClient.v.ToString();
      args[i + 5] = otherClient.hp.ToString();
      i += 6;
    }

    BrawlServer.Send(client, "List", args);
  }

  public static void Move(ClientState client, string[] args) {
    client.x = float.Parse(args[1]);
    client.y = float.Parse(args[2]);
    client.z = float.Parse(args[3]);
    BrawlServer.SendToOther(client, "Move", args);
  }

  public static void Attack(ClientState client, string[] args) {
    BrawlServer.SendToOther(client, "Attack", args);
  }

  public static void Hit(ClientState client, string[] args) {
    ClientState? hitClient = null;
    foreach (ClientState otherClient in BrawlServer.clients.Values) {
      if (otherClient.socket.RemoteEndPoint.ToString() == args[1]) {
        hitClient = otherClient;
      }
    }
    if (hitClient == null) {
      return;
    }
    hitClient.hp -= 25;
    if (hitClient.hp <= 0) {
      Thread.Sleep(50);
      foreach (ClientState otherClient in BrawlServer.clients.Values) {
        BrawlServer.Send(otherClient, "Die", new string[] {
          hitClient.socket.RemoteEndPoint.ToString()
        });
      }
    }
  }
}
