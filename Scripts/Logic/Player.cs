public class Player(ClientState state) {
  public readonly ClientState state = state;
  public PlayerData data;

  public string id = "";

  public void Send(MsgBase msg) {
    NetManager.Send(state, msg);
  }
}
