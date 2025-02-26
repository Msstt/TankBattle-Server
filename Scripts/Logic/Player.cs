public class Player(ClientState state) {
  public readonly ClientState state = state;
  public PlayerData data;

  public string id = "";
  public int roomId = -1;
  public int camp = 0;

  public void Send(MsgBase msg) {
    NetManager.Send(state, msg);
  }
}
