using System.Net.Sockets;

public class ClientState {
  public Socket socket;
  public ByteArray readBuffer = new();

  public long lastPingTime;

  public Player player;
}
