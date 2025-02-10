
using System.Net;
using System.Net.Sockets;


class MainClass {
  static void Main() {
    Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));
    socket.Listen(0);
    Console.WriteLine("Socket listen success!");
    while (true) {
      Socket client = socket.Accept();
      Console.WriteLine("Accept client success!");
      byte[] buffer = new byte[1024];
      int bytesCount = client.Receive(buffer);
      string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesCount);
      Console.WriteLine("Receive message: " + message);
      client.Send(System.Text.Encoding.UTF8.GetBytes(message));
      client.Close();
    }
  }
}
