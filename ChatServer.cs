
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;

class ChatServer {
  class ClientState {
    public required Socket socket;
    public byte[] buffer = new byte[1024];
  }

  private static readonly Dictionary<Socket, ClientState> clients = [];
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
      socket.Close();
      clients.Remove(socket);
      Console.WriteLine("Socket receive failed: " + ex);
      return false;
    }
    if (bytesCount == 0) {
      socket.Close();
      clients.Remove(socket);
      Console.WriteLine("Client close.");
      return false;
    }
    string message = System.Text.Encoding.UTF8.GetString(clients[socket].buffer, 0, bytesCount);
    Console.WriteLine("Receive message: " + message);
    // byte[] buffer = System.Text.Encoding.UTF8.GetBytes(socket.RemoteEndPoint.ToString() + ": " + message);
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
    foreach (Socket client in clients.Keys) {
      client.Send(buffer);
    }
    return true;
  }
}
