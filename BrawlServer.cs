
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.VisualBasic;

public class ClientState {
  public required Socket socket;
  public byte[] buffer = new byte[1024];

  public float x;
  public float y;
  public float z;
  public float v;
  public float hp = 100;
}

class BrawlServer {
  public static readonly Dictionary<Socket, ClientState> clients = [];
  public static void Run() {
    Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));
    socket.Listen(0);
    Console.WriteLine("Socket listen success!");
    while (true) {
      List<Socket> list = [];
      list.Add(socket);
      foreach (Socket client in clients.Keys) {
        list.Add(client);
      }
      Socket.Select(list, null, null, 1000);
      foreach (Socket item in list) {
        if (item == socket) {
          ReadListenSocket(item);
        } else {
          ReadClientSocket(item);
        }
      }
    }
  }

  private static void ReadListenSocket(Socket socket) {
    Socket client = socket.Accept();
    clients.Add(client, new ClientState {
      socket = client
    });
    Console.WriteLine("Accept client success!");
  }

  private static bool ReadClientSocket(Socket socket) {
    int bytesCount = 0;
    try {
      bytesCount = socket.Receive(clients[socket].buffer);
    } catch (SocketException ex) {
      Console.WriteLine("Socket receive failed: " + ex);
      Close(socket);
      return false;
    }
    if (bytesCount == 0) {
      Close(socket);
      return false;
    }
    string message = System.Text.Encoding.UTF8.GetString(clients[socket].buffer, 0, bytesCount);
    Console.WriteLine("Receive message: " + message);

    string[] msgArray = message.Split('|');
    MethodInfo? method = typeof(MsgHandler).GetMethod(msgArray[0]);
    method?.Invoke(null, [
        clients[socket], msgArray.Length > 1 ? msgArray[1].Split(',') : []
      ]);
    return true;
  }

  public static void Send(ClientState client, string topic, string[] args) {
    string message = topic + "|";
    foreach (string arg in args) {
      message += arg + ",";
    }
    if (args.Length > 0) {
      message = message[..^1];
    }
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
    client.socket.Send(buffer);
  }

  public static void SendToOther(ClientState client, string topic, string[] args) {
    foreach (ClientState otherClient in clients.Values) {
      if (otherClient == client) {
        continue;
      }
      Send(otherClient, topic, args);
    }
  }

  private static void Close(Socket socket) {
    MethodInfo? method = typeof(EventHandler).GetMethod("OnDisconnect");
    method?.Invoke(null, [
        clients[socket]
      ]);

    socket.Close();
    clients.Remove(socket);
    Console.WriteLine("Client close.");
  }
}
