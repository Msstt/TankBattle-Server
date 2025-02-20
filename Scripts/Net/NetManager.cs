using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

public class NetManager {
  public static long pingInterval = 30;
  private static readonly List<string> systemMsg = ["MsgPing"];

  private static Socket socket;
  public static readonly Dictionary<Socket, ClientState> clients = [];
  private static List<Socket> checkSockets = [];

  public static void StartLoop(string ip, int port) {
    socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
    socket.Listen(0);
    Console.WriteLine("NetManager start listen.");
    while (true) {
      ResetList();
      Socket.Select(checkSockets, null, null, 1000);
      foreach (Socket readySocket in checkSockets) {
        if (readySocket == socket) {
          ReadListenFd(readySocket);
        } else {
          ReadClientFd(readySocket);
        }
      }
      Timer();
    }
  }

  private static void ResetList() {
    checkSockets.Clear();
    checkSockets.Add(socket);
    foreach (Socket client in clients.Keys) {
      checkSockets.Add(client);
    }
  }

  private static void Timer() {
    MethodInfo? method = typeof(EventHandler).GetMethod("OnTimer");
    method?.Invoke(null, []);
  }

  public static long GetTimeStamp() {
    TimeSpan time = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
    return Convert.ToInt64(time.TotalSeconds);
  }

  #region 建立连接
  private static void ReadListenFd(Socket socket) {
    try {
      Socket client = socket.Accept();
      ClientState state = new() {
        socket = client,
        lastPingTime = GetTimeStamp()
      };
      clients.Add(client, state);
      Console.WriteLine("NetManager accept " + client.RemoteEndPoint.ToString() + ".");
    } catch (SocketException ex) {
      Console.WriteLine("Socket accept failed: " + ex + "!");
    }
  }
  #endregion

  #region 接收数据
  private static void ReadClientFd(Socket socket) {
    ByteArray readBuffer = clients[socket].readBuffer;
    if (readBuffer.Remain <= 0) {
      OnReceiveData(clients[socket]);
      readBuffer.Move();
    }
    if (readBuffer.Remain <= 0) {
      Console.WriteLine("NetManager receive failed because msg length > buffer capacity!");
      Close(clients[socket]);
      return;
    }
    int count;
    try {
      count = socket.Receive(readBuffer.buffer, readBuffer.writeIndex, readBuffer.Remain, 0);
    } catch (SocketException ex) {
      Console.WriteLine("Socket receive failed" + ex + "!");
      Close(clients[socket]);
      return;
    }
    if (count == 0) {
      Close(clients[socket]);
      return;
    }
    readBuffer.writeIndex += count;
    OnReceiveData(clients[socket]);
  }

  private static void OnReceiveData(ClientState state) {
    ByteArray readBuffer = state.readBuffer;
    if (readBuffer.Length < 2) {
      return;
    }
    Int16 length = readBuffer.ReadInt16();
    if (readBuffer.Length < 2 + length) {
      return;
    }
    readBuffer.readIndex += 2;
    string msgName = MsgBase.DecodeName(readBuffer.buffer, readBuffer.readIndex, out int readCount);
    if (msgName == "" || readCount > length) {
      Console.WriteLine("Msg Decode name failed!");
      return;
    }
    if (!systemMsg.Contains(msgName)) {
      Console.WriteLine("Receive message from " + state.socket.RemoteEndPoint.ToString() + ": " + msgName + ".");
    }
    MsgBase? msg = MsgBase.Decode(msgName, readBuffer.buffer, readBuffer.readIndex + readCount, length - readCount);
    readBuffer.readIndex += length;
    readBuffer.CheckAndMove();
    MethodInfo? method = typeof(MsgHandler).GetMethod(msgName);
    if (method != null) {
      method?.Invoke(null, [state, msg]);
    } else {
      Console.WriteLine(msgName + "'s handler missing!");
    }
    OnReceiveData(state);
  }
  #endregion

  #region 发送数据
  public static void Send(ClientState state, MsgBase msg) {
    if (!state.socket.Connected) {
      return;
    }
    byte[] nameBytes = MsgBase.EncodeName(msg);
    byte[] bodyBytes = MsgBase.Encode(msg);
    int length = nameBytes.Length + bodyBytes.Length;
    byte[] sendBytes = new byte[2 + length];
    sendBytes[0] = (byte)(length % 256);
    sendBytes[1] = (byte)(length / 256);
    Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
    Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
    try {
      state.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
    } catch (SocketException ex) {
      Console.WriteLine("Socket send failed: " + ex + "!");
    }
  }
  #endregion

  #region 关闭连接
  public static void Close(ClientState? state) {
    if (state == null) {
      return;
    }
    MethodInfo? method = typeof(EventHandler).GetMethod("OnDisconnect");
    method?.Invoke(null, [state]);
    string endPoint = state.socket.RemoteEndPoint.ToString();
    state.socket.Close();
    clients.Remove(state.socket);
    Console.WriteLine("Socket close: " + endPoint + ".");
  }
  #endregion
}
