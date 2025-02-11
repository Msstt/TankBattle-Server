
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;

class ClientState {
  public required Socket socket;
  public byte[] buffer = new byte[1024];
}

class MainClass {
  private static readonly Dictionary<Socket, ClientState> clients = [];
  static void Main() {
    Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));
    socket.Listen(0);
    Console.WriteLine("Socket listen success!");
    socket.BeginAccept(OnAccept, socket);
    while (true) {
    }
  }

  private static void OnAccept(IAsyncResult ar) {
    try {
      Socket socket = (Socket)ar.AsyncState;
      Socket client = socket.EndAccept(ar);
      clients.Add(client, new ClientState {
        socket = client
      });
      client.BeginReceive(clients[client].buffer, 0, 1024, 0, OnReceive, client);
      socket.BeginAccept(OnAccept, socket);
      Console.WriteLine("Accept client success!");
    } catch (SocketException ex) {
      Console.WriteLine("Accept client failed: " + ex);
    }
  }

  private static void OnReceive(IAsyncResult ar) {
    try {
      Socket socket = (Socket)ar.AsyncState;
      int bytesCount = socket.EndReceive(ar);
      if (bytesCount == 0) {
        socket.Close();
        clients.Remove(socket);
        Console.WriteLine("Client close.");
        return;
      }
      string message = System.Text.Encoding.UTF8.GetString(clients[socket].buffer, 0, bytesCount);
      Console.WriteLine("Receive message: " + message);
      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(socket.RemoteEndPoint.ToString() + ": " + message);
      foreach (Socket client in clients.Keys) {
        client.BeginSend(buffer, 0, buffer.Length, 0, OnSend, client);
      }
      socket.BeginReceive(clients[socket].buffer, 0, 1024, 0, OnReceive, socket);
    } catch (SocketException ex) {
      Console.WriteLine("Socket receive failed: " + ex);
    }
  }

  private static void OnSend(IAsyncResult ar) {
    try {
      Socket socket = (Socket)ar.AsyncState;
      int bytesCount = socket.EndSend(ar);
      Console.WriteLine("Socket send " + bytesCount + " bytes success.");
    } catch (SocketException ex) {
      Console.WriteLine("Socket send failed!" + ex);
    }
  }
}
